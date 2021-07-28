namespace PaymentSystem.WalletApp.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using PaymentSystem.WalletApp.Data.Common.Models;

    public class Activity : BaseDeletableModel<string>
    {
        public Activity()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Required]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public double Amount { get; set; }

        [Required]
        public string CounterpartyAddress { get; set; }

        public string Description { get; set; }

        public ActivityStatus Status { get; set; }

        public long TimeStamp { get; set; }
    }
}
