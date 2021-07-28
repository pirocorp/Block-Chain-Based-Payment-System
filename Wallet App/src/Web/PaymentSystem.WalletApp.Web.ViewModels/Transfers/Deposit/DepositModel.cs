namespace PaymentSystem.WalletApp.Web.ViewModels.Transfers.Deposit
{
    using System.ComponentModel.DataAnnotations;

    using PaymentSystem.WalletApp.Services.Data.Models;

    public class DepositModel
    {
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be 1 or more.")]
        public double Amount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
    }
}
