namespace PaymentSystem.BlockChain.Services.Data.Models
{
    public class AccountServiceModel
    {
        public string Address { get; set; }

        public string PublicKey { get; set; }

        public string Secret { get; set; }

        public double Balance { get; set; }
    }
}
