namespace PaymentSystem.BlockChain.Data.Models
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class Account : IEntityTypeConfiguration<Account>
    {
        public string Address { get; set; }

        public double Balance { get; set; }

        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasKey(a => a.Address);
        }
    }
}
