namespace PaymentSystem.WalletApp.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using PaymentSystem.WalletApp.Data.Common.Models;

    using static Data.Common.DataConstants;
    using static Data.Common.DataConstants.BankAccount;

    public class BankAccount : BaseDeletableModel<string>
    {
        [StringLength(CountryLength)]
        public string Country { get; set; }

        [StringLength(BankNameLength)]
        public string BankName { get; set; }

        [StringLength(AccountHolderNameLength)]
        public string AccountHolderName { get; set; }

        [StringLength(IBANLength)]
        public string IBAN { get; set; }

        [StringLength(SwiftLength)]
        public string Swift { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}
