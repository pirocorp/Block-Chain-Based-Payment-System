namespace PaymentSystem.WalletApp.Data.Models
{
    using PaymentSystem.WalletApp.Data.Common.Models;

    public class Beneficiary : BaseModel<string>
    {
        public string Address { get; set; }

        public string PublicKey { get; set; }

        public string Name { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}
