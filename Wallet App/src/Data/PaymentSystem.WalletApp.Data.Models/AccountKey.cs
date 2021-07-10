namespace PaymentSystem.WalletApp.Data.Models
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using PaymentSystem.WalletApp.Data.Common.Models;

    public class AccountKey : BaseModel<string>, IEntityTypeConfiguration<AccountKey>
    {
        public string Address { get; set; }

        public Account Account { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        /// <summary>
        /// Gets or Sets, symmetrically encrypted key with password provided from user.
        /// Key is used to be restored Private public key pair
        /// for signing transactions.
        /// </summary>
        public string Key { get; set; }

        public void Configure(EntityTypeBuilder<AccountKey> builder)
        {
            builder
                .HasOne(ak => ak.Account)
                .WithOne()
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
