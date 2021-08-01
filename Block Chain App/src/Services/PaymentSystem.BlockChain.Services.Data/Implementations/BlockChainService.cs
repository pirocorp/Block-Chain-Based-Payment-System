namespace PaymentSystem.BlockChain.Services.Data.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Common.Utilities;
    using Microsoft.EntityFrameworkCore;

    using PaymentSystem.BlockChain.Data;
    using PaymentSystem.Common;
    using PaymentSystem.Common.Data.Models;

    public class BlockChainService : IBlockChainService
    {
        private readonly SystemKeys system;
        private readonly ApplicationDbContext context;
        private readonly ITransactionPool transactionPool;
        private readonly ICancelTransactionPool cancelTransactionPool;
        private readonly IAccountService accountService;

        public BlockChainService(
            SystemKeys system,
            ApplicationDbContext context,
            ITransactionPool transactionPool,
            ICancelTransactionPool cancelTransactionPool,
            IAccountService accountService)
        {
            this.system = system;
            this.context = context;
            this.transactionPool = transactionPool;
            this.cancelTransactionPool = cancelTransactionPool;
            this.accountService = accountService;
        }

        public async Task<int> Count()
            => await this.context.Blocks.CountAsync();

        public async Task<Block> GetLastBlock()
            => await this.context.Blocks
                .OrderByDescending(x => x.Height)
                .Include(x => x.Transactions)
                .FirstAsync();

        public async Task<Block> GetGenesisBlock()
            => await this.context.Blocks
                .Include(x => x.Transactions)
                .FirstAsync(x => x.Height == 0);

        public async Task<Block> GetBlockByHash(string hash)
            => await this.context.Blocks
                .Include(x => x.Transactions)
                .FirstOrDefaultAsync(x => x.Hash.Equals(hash));

        public async Task<Block> GetBlockByHeight(long height)
            => await this.context.Blocks
                .Include(b => b.Transactions)
                .FirstOrDefaultAsync(b => b.Height.Equals(height));

        public async Task<IEnumerable<Block>> GetBlocks(int pageNumber, int resultPerPage)
            => await this.context.Blocks
                .OrderBy(x => x.Height)
                .Skip((pageNumber - 1) * resultPerPage)
                .Take(resultPerPage)
                .Include(x => x.Transactions)
                .ToListAsync();

        public void AddTransaction(Transaction transaction)
            => this.transactionPool.Add(transaction);

        public async IAsyncEnumerable<Block[]> GetBlockChain(long height)
        {
            var query = this.context.Blocks
                .Where(x => x.Height <= height)
                .OrderBy(x => x.Height)
                .Include(x => x.Transactions);

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
                    Validator = this.system.Address,
                },

                Height = nextHeight,
                Transactions = validTransactions,
            };

            block.BlockHeader.MerkleRoot = BlockChainHashing.GenerateMerkleRoot(validTransactions);
            block.Hash = BlockChainHashing.GenerateBlockHash(block);

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

                await this.accountService.TryDeposit(this.system.Address, transaction.Fee);

                validTransactions.Add(transaction);
            }

            return validTransactions;
        }
    }
}
