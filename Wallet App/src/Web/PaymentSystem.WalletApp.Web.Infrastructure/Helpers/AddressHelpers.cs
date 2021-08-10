namespace PaymentSystem.WalletApp.Web.Infrastructure.Helpers
{
    using System.Linq;

    public static class AddressHelpers
    {
        private const int ChunkSize = 8;

        public static string FriendlyAddress(string address)
            => string.Join("-", Enumerable
                .Range(0, address.Length / ChunkSize)
                .Select(i => address.Substring(i * ChunkSize, ChunkSize)));
    }
}
