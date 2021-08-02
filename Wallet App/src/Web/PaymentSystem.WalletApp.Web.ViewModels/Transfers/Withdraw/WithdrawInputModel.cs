namespace PaymentSystem.WalletApp.Web.ViewModels.Transfers.Withdraw
{
    using System.ComponentModel.DataAnnotations;

    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Services.Data.Models.Transactions;

    using static Infrastructure.WebConstants.WithdrawInputModel;

    public class WithdrawInputModel : IMapTo<WithdrawServiceModel>
    {
        [Required]
        public string BankAccount { get; set; }

        [Required]
        public string CoinAccount { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = AmountErrorMessage)]
        public double Amount { get; set; }
    }
}
