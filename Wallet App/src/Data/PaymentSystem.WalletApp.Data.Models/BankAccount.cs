namespace PaymentSystem.WalletApp.Data.Models
{
    using PaymentSystem.WalletApp.Data.Common.Models;

    public class BankAccount : BaseDeletableModel<string>
    {
        public string Country { get; set; }

        public string BankName { get; set; }

        public string AccountHolderName { get; set; }

        public string IBAN { get; set; }

        public string Swift { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}
