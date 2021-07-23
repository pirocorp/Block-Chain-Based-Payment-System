namespace PaymentSystem.WalletApp.Services.Implementations
{
    using System.Security.Cryptography;

    using PaymentSystem.Common.Utilities;

    public class SaltService : ISaltService
    {
        private const int MaxSaltLength = 32;

        public byte[] GetSalt()
        {
            var random = new RNGCryptoServiceProvider();

            // Empty salt array
            var salt = new byte[MaxSaltLength];

            // Build the random bytes
            random.GetNonZeroBytes(salt);

            return salt;
        }

        public string GetSaltHex()
            => this.GetSalt().BytesToHex();
    }
}
