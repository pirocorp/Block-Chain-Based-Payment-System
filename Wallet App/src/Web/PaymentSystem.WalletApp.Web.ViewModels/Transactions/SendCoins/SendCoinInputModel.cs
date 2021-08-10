namespace PaymentSystem.WalletApp.Web.ViewModels.Transactions.SendCoins
{
    using System.ComponentModel.DataAnnotations;

    using static Infrastructure.WebConstants.SendCoinErrorMessages;

    public class SendCoinInputModel
    {
        [Required]
        public string CoinAccount { get; set; }

        public bool HasKey { get; set; }

        [Required]
        public string Recipient { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = AmountErrorMessage)]
        public double Amount { get; set; }
    }
}
