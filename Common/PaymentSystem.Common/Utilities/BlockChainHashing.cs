namespace PaymentSystem.Common.Utilities
{
    using System.Security.Cryptography;
    using System.Text;

    public static class BlockChainHashing
    {
        public static string GenerateHash(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            var hash = SHA256.Create().ComputeHash(bytes);

            return hash.BytesToHex();
        }
    }
}
