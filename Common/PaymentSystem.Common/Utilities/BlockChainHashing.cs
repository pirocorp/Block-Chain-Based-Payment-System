namespace PaymentSystem.Common.Utilities
{
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
        {
            var data = input.TimeStamp
                       + input.Sender
                       + input.Recipient
                       + input.Amount
                       + input.Fee;

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
    }
}
