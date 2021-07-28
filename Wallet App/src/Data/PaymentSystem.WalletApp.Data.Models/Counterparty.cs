namespace PaymentSystem.WalletApp.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using PaymentSystem.WalletApp.Data.Common.Models;

    using static Data.Common.DataConstants.Beneficiary;

    public class Counterparty : BaseModel<string>
    {
        [Required]
        public string Address { get; set; }

        [Required]
        public string PublicKey { get; set; }

        [StringLength(NameLength)]
        public string Name { get; set; }

        [Required]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}
