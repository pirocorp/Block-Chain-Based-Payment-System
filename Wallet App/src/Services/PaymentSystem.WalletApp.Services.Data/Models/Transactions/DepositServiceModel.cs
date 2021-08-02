namespace PaymentSystem.WalletApp.Services.Data.Models.Transactions
{
    public class DepositServiceModel
    {
        public double Amount { get; set; }

        public double Fee { get; set; }

        public string RecipientAddress { get; set; }
    }
}
