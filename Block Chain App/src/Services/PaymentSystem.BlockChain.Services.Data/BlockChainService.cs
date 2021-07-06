﻿namespace PaymentSystem.BlockChain.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;

    using PaymentSystem.BlockChain.Data;
    using PaymentSystem.Common;
    using PaymentSystem.Common.Data.Models;
    using PaymentSystem.Common.Utilities;

    public class BlockChainService : IBlockChainService
    {
        private readonly ApplicationDbContext context;
        private readonly ITransactionPool transactionPool;
        private readonly ICancelTransactionPool cancelTransactionPool;
        private readonly IAccountService accountService;
        private readonly ITransactionService transactionService;

        public BlockChainService(
            ApplicationDbContext context,
            ITransactionPool transactionPool,
            ICancelTransactionPool cancelTransactionPool,
            IAccountService accountService,
            ITransactionService transactionService)
        {
            this.context = context;
            this.transactionPool = transactionPool;
            this.cancelTransactionPool = cancelTransactionPool;
            this.accountService = accountService;
            this.transactionService = transactionService;
        }

        public async Task<int> Count()
            => await this.context.Blocks.CountAsync();

        public async Task<Block> GetLastBlock()
            => await this.context.Blocks.OrderByDescending(x => x.Height).FirstAsync();

        public async Task<Block> GetGenesisBlock()
            => await this.context.Blocks
                .Include(x => x.Transactions)
                .FirstAsync(x => x.Height == 0);

        public async Task<Block> GetBlockByHash(string hash)
            => await this.context.Blocks.FirstOrDefaultAsync(x => x.Hash.Equals(hash));

        public async Task<Block> GetBlockByHeight(long height)
            => await this.context.Blocks.FirstOrDefaultAsync(b => b.Height.Equals(height));

        public async Task<IEnumerable<Block>> GetBlocks(int pageNumber, int resultPerPage)
            => await this.context.Blocks
                .OrderByDescending(x => x.Height)
                .Skip((pageNumber - 1) * resultPerPage)
                .Take(resultPerPage)
                .ToListAsync();

        public void AddTransaction(Transaction transaction)
            => this.transactionPool.Add(transaction);

        public async IAsyncEnumerable<Block[]> GetBlockChain(long height)
        {
            var query = this.context.Blocks
                .Where(x => x.Height <= height)
                .OrderByDescending(x => x.Height);

            var batchPage = 1;
            var batchSize = GlobalConstants.BlockChainBatchSize;

            var batch = await query
                .Take(batchSize)
                .ToArrayAsync();

            while (batch.Length > 0)
            {
                yield return batch;

                batchPage++;
                batch = await query
                    .Skip((batchPage - 1) * batchSize)
                    .Take(batchSize)
                    .ToArrayAsync();
            }
        }

        public async Task<Block> MineBlock()
        {
            var transactions = this.transactionPool.GetAll().ToList();

            // If there are no new transactions there are no reason to mine new block.
            // The state of the system remains the same.
            if (transactions.Count == 0)
            {
                return null;
            }

            var lastBlock = await this.GetLastBlock();
            var previousHash = lastBlock.Hash;
            var nextHeight = lastBlock.Height + 1;

            var validTransactions = await this.ValidateAndProcessTransactions(transactions);

            var block = new Block
            {
                BlockHeader =
                {
                    Version = GlobalConstants.Block.Version,
                    PreviousHash = previousHash,
                    TimeStamp = DateTime.UtcNow.Ticks,
                    Difficulty = GlobalConstants.Block.DefaultDifficulty,
                    Validator = GlobalConstants.Block.DefaultValidator,
                },

                Height = nextHeight,
                Transactions = validTransactions,
            };

            block.BlockHeader.MerkleRoot = BlockHelpers.GenerateMerkleRoot(validTransactions);
            block.Hash = BlockHelpers.GenerateBlockHash(block);

            validTransactions.ForEach(t => t.BlockHash = block.Hash);

            await this.context.AddAsync(block);
            await this.context.SaveChangesAsync();

            return block;
        }

        private async Task<List<Transaction>> ValidateAndProcessTransactions(IEnumerable<Transaction> transactions)
        {
            var validTransactions = new List<Transaction>();

            foreach (var transaction in transactions)
            {
                var totalAmount = transaction.Amount + transaction.Fee;
                var success = await this.accountService.TryWithdraw(transaction.Sender, totalAmount);

                if (!success)
                {
                    this.cancelTransactionPool.Add(transaction);
                    continue;
                }

                var depositIsSuccessful = await this.accountService.TryDeposit(transaction.Recipient, transaction.Amount);

                if (!depositIsSuccessful)
                {
                    await this.accountService.TryDeposit(transaction.Sender, transaction.Amount);
                    this.cancelTransactionPool.Add(transaction);
                    continue;
                }

                await this.accountService.TryDeposit(GlobalConstants.Block.DefaultValidator, transaction.Fee);

                validTransactions.Add(transaction);
            }



            return validTransactions;
        }
    }
}
