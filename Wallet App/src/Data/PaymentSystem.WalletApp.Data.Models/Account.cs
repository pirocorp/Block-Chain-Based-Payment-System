namespace PaymentSystem.WalletApp.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using PaymentSystem.WalletApp.Data.Common.Models;

    using static Common.DataConstants.Account;

    public class Account : IAuditInfo
    {
        [Key]
        public string Address { get; set; }

        /// <summary>
        /// Payments, Savings, etc.
        /// </summary>
        [StringLength(NameLength)]
        public string Name { get; set; }

        public string PublicKey { get; set; }

        [Range(0, double.MaxValue)]
        public double Balance { get; set; }

        [Range(0, double.MaxValue)]
        public double BlockedBalance { get; set; }

        [Required]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public AccountKey AccountKey { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}
