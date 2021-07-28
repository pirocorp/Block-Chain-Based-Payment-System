namespace PaymentSystem.WalletApp.Services.Data.Models.Transactions
{
    public class CreateTransactionServiceModel
    {
        public double Amount { get; set; }

        public string RecipientAddress { get; set; }
    }
}
