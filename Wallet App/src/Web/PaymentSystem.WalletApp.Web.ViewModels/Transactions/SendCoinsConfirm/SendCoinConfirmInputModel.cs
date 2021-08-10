namespace PaymentSystem.WalletApp.Web.ViewModels.Transactions.SendCoinsConfirm
{
    using System.ComponentModel.DataAnnotations;

    using static Infrastructure.WebConstants.SendCoinErrorMessages;

    public class SendCoinConfirmInputModel
    {
        [Required]
        public string CoinAccount { get; set; }

        public string Secret { get; set; }

        [Required]
        public string Recipient { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = AmountErrorMessage)]
        public double Amount { get; set; }
    }
}
