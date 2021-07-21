namespace PaymentSystem.WalletApp.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using PaymentSystem.WalletApp.Data.Common.Models;

    using static Data.Common.DataConstants;
    using static Data.Common.DataConstants.Beneficiary;

    public class Beneficiary : BaseModel<string>
    {
        public string Address { get; set; }

        public string PublicKey { get; set; }

        [StringLength(NameLength)]
        public string Name { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}
