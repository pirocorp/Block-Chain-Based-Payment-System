namespace PaymentSystem.WalletApp.Web.ViewModels.Transactions.SendCoinsConfirm
{
    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Common;
    using PaymentSystem.WalletApp.Web.ViewModels.Transactions.SendCoins;

    public class SendCoinsConfirmViewModel : IMapFrom<SendCoinInputModel>
    {
        public string CoinAccount { get; set; }

        public bool HasKey { get; set; }

        public string Recipient { get; set; }

        public double Amount { get; set; }

        public double Fees => this.Amount * (WalletConstants.DefaultFeePercent / 100);

        public double TotalAmount => this.Amount + this.Fees;
    }
}
