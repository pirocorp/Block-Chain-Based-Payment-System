namespace PaymentSystem.WalletApp.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using PaymentSystem.WalletApp.Data.Common.Models;

    using static Common.DataConstants.Activity;

    public class Activity : BaseDeletableModel<string>, IEntityTypeConfiguration<Activity>
    {
        public Activity()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Required]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public double Amount { get; set; }

        public double BlockedAmount { get; set; }

        [Required]
        public string CounterpartyAddress { get; set; }

        [StringLength(DescriptionLength)]
        public string Description { get; set; }

        public ActivityStatus Status { get; set; }

        public long TimeStamp { get; set; }

        [Required]
        [StringLength(TransactionHashLength)]
        public string TransactionHash { get; set; }

        public void Configure(EntityTypeBuilder<Activity> builder)
        {
            builder
                .HasIndex(b => b.TransactionHash);
        }
    }
}
