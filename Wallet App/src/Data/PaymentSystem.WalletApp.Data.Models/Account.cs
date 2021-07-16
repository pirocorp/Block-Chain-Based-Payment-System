namespace PaymentSystem.WalletApp.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using PaymentSystem.Common.Data.Models;
    using PaymentSystem.WalletApp.Data.Common.Models;

    public class Account : IAuditInfo, IEntityTypeConfiguration<Account>
    {
        public Account()
        {
            this.OutboundTransactions = new HashSet<Transaction>();
            this.InboundTransactions = new HashSet<Transaction>();
        }

        /// <summary>
        /// Payments, Savings, etc.
        /// </summary>
        public string Name { get; set; }

        public string Address { get; set; }

        public string PublicKey { get; set; }

        public double Balance { get; set; }

        [Required]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public ICollection<Transaction> OutboundTransactions { get; set; }

        public ICollection<Transaction> InboundTransactions { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasKey(a => a.Address);

            builder
                .HasMany(a => a.OutboundTransactions)
                .WithOne()
                .HasForeignKey(t => t.Sender)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(a => a.InboundTransactions)
                .WithOne()
                .HasForeignKey(t => t.Recipient)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
