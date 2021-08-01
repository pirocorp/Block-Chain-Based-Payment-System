namespace PaymentSystem.WalletApp.Data.Models.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using PaymentSystem.Common.Data.Models;

    public class TransactionsConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasIndex(t => t.Sender);
            builder.HasIndex(t => t.Recipient);
        }
    }
}
