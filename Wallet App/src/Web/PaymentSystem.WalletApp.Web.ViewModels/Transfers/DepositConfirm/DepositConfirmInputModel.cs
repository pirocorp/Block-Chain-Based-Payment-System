namespace PaymentSystem.WalletApp.Web.ViewModels.Transfers.DepositConfirm
{
    using System.ComponentModel.DataAnnotations;

    using PaymentSystem.WalletApp.Services.Data.Models;

    using static PaymentSystem.WalletApp.Web.Infrastructure.WebConstants.DepositConfirmInputModel;

    public class DepositConfirmInputModel
    {
        [Required]
        [StringLength(PaymentMethodMaxLength, MinimumLength = PaymentMethodMinLength)]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }

        [Required]
        [StringLength(AccountMaxLength, MinimumLength = AccountMinLength)]
        public string Account { get; set; }

        [Range(1, double.MaxValue)]
        public double Amount { get; set; }

        public PaymentMethod PaymentType { get; set; }
    }
}
