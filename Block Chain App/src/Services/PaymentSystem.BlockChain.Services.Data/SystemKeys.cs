namespace PaymentSystem.BlockChain.Services.Data
{
    using PaymentSystem.Common.Utilities;

    public class SystemKeys : AccountKeys
    {
        public SystemKeys()
        {
        }

        public void Update(string secret) => this.PrivateKey = new AccountKeys(secret).PrivateKey;
    }
}
