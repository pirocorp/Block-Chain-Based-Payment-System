namespace PaymentSystem.WalletApp.Web.ViewModels.Transfers
{
    public class DepositModel
    {
        public double Amount { get; set; }

        public DepositModelPaymentMethod PaymentMethod { get; set; }
    }
}
