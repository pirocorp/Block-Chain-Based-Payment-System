namespace PaymentSystem.WalletApp.Services.Data.Models.Transactions
{
    public class WithdrawServiceModel
    {
        public string BankAccount { get; set; }

        public string CoinAccount { get; set; }

        public double Amount { get; set; }
    }
}
