namespace PaymentSystem.WalletApp.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using PaymentSystem.WalletApp.Data.Common.Models;

    using static Data.Common.DataConstants;

    public class AccountKey : BaseModel<string>, IEntityTypeConfiguration<AccountKey>
    {
        public AccountKey()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Address { get; set; }

        public Account Account { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        [StringLength(SecurityStampLength)]
        public string SecurityStamp { get; set; }

        /// <summary>
        /// Gets or Sets, symmetrically encrypted key.
        /// Key is used to be restored Private public key pair
        /// for signing transactions.
        /// </summary>
        public string Key { get; set; }

        public void Configure(EntityTypeBuilder<AccountKey> builder)
        {
            builder
                .HasOne(ak => ak.Account)
                .WithOne(a => a.AccountKey)
                .HasForeignKey<AccountKey>(ak => ak.Address)
                .IsRequired();

            builder
                .HasOne(ak => ak.User)
                .WithMany(u => u.AccountKeys)
                .HasForeignKey(ak => ak.UserId)
                .IsRequired();
        }
    }
}
