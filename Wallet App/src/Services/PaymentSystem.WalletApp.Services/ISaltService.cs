namespace PaymentSystem.WalletApp.Services
{
    public interface ISaltService
    {
        public byte[] GetSalt();

        public string GetSaltHex();
    }
}
