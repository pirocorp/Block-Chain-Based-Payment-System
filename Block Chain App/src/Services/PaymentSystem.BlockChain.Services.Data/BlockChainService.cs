namespace PaymentSystem.BlockChain.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;

    using PaymentSystem.BlockChain.Data;
    using PaymentSystem.Common;
    using PaymentSystem.Common.Data.Models;
    using PaymentSystem.Common.Utilities;

    public class BlockChainService : IBlockChainService
    {
        private readonly ApplicationDbContext context;
        private readonly ITransactionPool transactionPool;

        public BlockChainService(
            ApplicationDbContext context,
            ITransactionPool transactionPool)
        {
            this.context = context;
            this.transactionPool = transactionPool;
        }

        public int Count()
            => this.context.Blocks.Count();

        public Block GetLastBlock()
            => this.context.Blocks.OrderByDescending(x => x.Height).First();

        public Block GetGenesisBlock()
            => this.context.Blocks.First(x => x.Height == 0);

        public Block GetBlockByHash(string hash)
            => this.context.Blocks.FirstOrDefault(x => x.Hash.Equals(hash));

        public Block GetBlockByHeight(long height)
            => this.context.Blocks.FirstOrDefault(b => b.Height.Equals(height));

        public IEnumerable<Block> GetBlocks(int pageNumber, int resultPerPage)
            => this.context.Blocks
                .OrderByDescending(x => x.Height)
                .Skip((pageNumber - 1) * resultPerPage)
                .Take(resultPerPage)
                .ToList();

        public void AddTransaction(Transaction transaction)
            => this.transactionPool.AddTransaction(transaction);

        public IEnumerable<IEnumerable<Block>> GetBlockChain(long height)
        {
            var query = this.context.Blocks
                .Where(x => x.Height <= height)
                .OrderByDescending(x => x.Height);

            var batchPage = 1;
            var batchSize = GlobalConstants.BlockChainBatchSize;

            var batch = query
                .Take(batchSize)
                .ToArray();

            while (batch.Length > 0)
            {
                yield return batch;

                batchPage++;
                batch = query
                    .Skip((batchPage - 1) * batchSize)
                    .Take(batchSize)
                    .ToArray();
            }
        }

        public void MineBlock()
        {
            var lastBlock = this.GetLastBlock();
            var previousHash = lastBlock.Hash;
            var transactions = this.transactionPool.GetTransactions().ToList();

            var nextHeight = lastBlock.Height + 1;

            var block = new Block
            {
                BlockHeader =
                {
                    Version = GlobalConstants.Block.Version,
                    PreviousHash = previousHash,
                    TimeStamp = DateTime.UtcNow.Ticks,
                    MerkleRoot = this.GenerateMerkleRoot(transactions),
                    Difficulty = GlobalConstants.Block.DefaultDifficulty,
                },

                Height = nextHeight,
                Transactions = transactions,
                Validator = GlobalConstants.Block.DefaultValidator,
            };

            block.Hash = this.GenerateBlockHash(block);

            this.context.Add(block);
        }

        private static string DoubleHash(string left, string right)
        {
            var leftByte = left.HexToBytes();
            var rightByte = right.HexToBytes();

            var concatHash = leftByte.Concat(rightByte).ToArray();
            var sha256 = SHA256.Create();
            var sendHash = sha256.ComputeHash(sha256.ComputeHash(concatHash));

            return sendHash.BytesToHex();
        }

        private string GenerateMerkleRoot(IList<Transaction> transactions)
        {
            var txsHash = transactions.Select(transaction => transaction.Hash).ToList();

            while (true)
            {
                if (txsHash.Count == 0)
                {
                    return string.Empty;
                }

                if (txsHash.Count == 1)
                {
                    return txsHash[0];
                }

                var newHashList = new List<string>();

                var length = (txsHash.Count % 2 != 0) ? txsHash.Count - 1 : txsHash.Count;

                for (var i = 0; i < length; i += 2)
                {
                    newHashList.Add(DoubleHash(txsHash[i], txsHash[i + 1]));
                }

                if (length < txsHash.Count)
                {
                    newHashList.Add(DoubleHash(txsHash[^1], txsHash[^1]));
                }

                txsHash = newHashList.ToList();
            }
        }

        private string GenerateBlockHash(Block block)
        {
            var blockData =
                block.BlockHeader.Version
                + block.BlockHeader.PreviousHash
                + block.BlockHeader.MerkleRoot
                + block.BlockHeader.TimeStamp
                + block.BlockHeader.Difficulty
                + block.Validator;

            return BlockChainHashing.GenerateHash(blockData);
        }
    }
}
