namespace PaymentSystem.BlockChain.Infrastructure
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using Common.Data.Models;
    using Common.Utilities;

    public class BlockHelpers
    {
        public static string GenerateBlockHash(Block block)
        {
            var blockData =
                block.BlockHeader.Version
                + block.BlockHeader.PreviousHash
                + block.BlockHeader.MerkleRoot
                + block.BlockHeader.TimeStamp
                + block.BlockHeader.Difficulty
                + block.BlockHeader.Validator
                + block.Height;

            return BlockChainHashing.GenerateHash(blockData);
        }

        public static string GenerateMerkleRoot(IEnumerable<Transaction> transactions)
        {
            var transactionsHashes = transactions.Select(transaction => transaction.Hash).ToList();

            while (true)
            {
                if (transactionsHashes.Count == 0)
                {
                    return string.Empty;
                }

                if (transactionsHashes.Count == 1)
                {
                    return transactionsHashes[0];
                }

                var newHashList = new List<string>();

                var length = (transactionsHashes.Count % 2 != 0) ? transactionsHashes.Count - 1 : transactionsHashes.Count;

                for (var i = 0; i < length; i += 2)
                {
                    newHashList.Add(DoubleHash(transactionsHashes[i], transactionsHashes[i + 1]));
                }

                if (length < transactionsHashes.Count)
                {
                    newHashList.Add(DoubleHash(transactionsHashes[^1], transactionsHashes[^1]));
                }

                transactionsHashes = newHashList.ToList();
            }
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
    }
}
