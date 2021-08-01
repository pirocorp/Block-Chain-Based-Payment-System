namespace PaymentSystem.Common.Utilities
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    using EllipticCurve;

    using PaymentSystem.Common.Data.Models;

    public static class BlockChainHashing
    {
        public static string GenerateHash(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            var hash = SHA256.Create().ComputeHash(bytes);

            return hash.BytesToHex();
        }

        public static string GenerateTransactionHash(Transaction input)
            => GenerateTransactionHash(
                input.TimeStamp,
                input.Sender,
                input.Recipient,
                input.Amount,
                input.Fee);

        public static string GenerateTransactionHash(long timeStamp, string sender, string recipient, double amount, double fee)
        {
            var data = timeStamp
                       + sender
                       + recipient
                       + amount
                       + fee;

            return GenerateHash(data);
        }

        /// <summary>
        /// This method validates that message is sign with private key for the public key.
        /// </summary>
        /// <param name="publicKeyHex">Public key for the wallet owner.</param>
        /// <param name="message">Message that will be validated.</param>
        /// <param name="signature">Provided signature of the owner.</param>
        /// <returns></returns>
        public static bool VerifySignature(string publicKeyHex, string message, string signature)
        {
            var byteArray = publicKeyHex.HexToBytes();
            var publicKey = PublicKey.fromString(byteArray);

            return Ecdsa.verify(message, Signature.fromBase64(signature), publicKey);
        }

        /// <summary>
        /// This method signs message with private key.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="secret">Secret to get private key.</param>
        /// <returns></returns>
        public static string CreateSignature(string message, string secret)
            => CreateSignature(message, AccountHelpers.RestoreAccount(secret).PrivateKey);

        /// <summary>
        /// This method signs message with private key.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="privateKey">Private key.</param>
        /// <returns></returns>
        public static string CreateSignature(string message, PrivateKey privateKey)
            => Ecdsa.sign(message, privateKey).toBase64();

        public static string GenerateBlockHash(Block block)
        {
            return GenerateBlockHash(
                block.BlockHeader.Version,
                block.BlockHeader.PreviousHash,
                block.BlockHeader.MerkleRoot,
                block.BlockHeader.TimeStamp,
                block.BlockHeader.Difficulty,
                block.BlockHeader.Validator,
                block.Height);
        }

        public static string GenerateBlockHash(int version, string previousHash, string merkleRoot, long timeStamp, int difficulty, string validator, long height)
        {
            var blockData =
                version
                + previousHash
                + merkleRoot
                + timeStamp
                + difficulty
                + validator
                + height;

            return BlockChainHashing.GenerateHash(blockData);
        }

        public static string GenerateMerkleRoot(IEnumerable<Transaction> transactions)
        {
            var transactionsHashes = transactions
                .Select(transaction => transaction.Hash).ToList();

            return GenerateMerkleRoot(transactionsHashes);
        }

        public static string GenerateMerkleRoot(IEnumerable<string> hashes)
        {
            var transactionsHashes = hashes.ToList();

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
