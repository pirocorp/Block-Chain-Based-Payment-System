namespace PaymentSystem.WalletApp.Web.ViewModels.Profile.Financials.Profile
{
    using System.ComponentModel.DataAnnotations;

    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Services.Data.Models.BankAccounts;

    using static Data.Common.DataConstants.BankAccount;

    public class AddBankAccountProfileModel : IMapTo<AddBankAccountServiceModel>
    {
        [Required]
        [StringLength(CountryLength)]
        public string Country { get; set; }

        [Required]
        [StringLength(BankNameLength)]
        public string BankName { get; set; }

        [Required]
        [StringLength(AccountHolderNameLength)]
        public string AccountName { get; set; }

        [Required]
        [StringLength(IBANLength, MinimumLength = IBANMinLength)]
        public string IBAN { get; set; }

        [Required]
        [StringLength(SwiftLength, MinimumLength = SwiftMinLength)]
        public string SWIFT { get; set; }
    }
}
