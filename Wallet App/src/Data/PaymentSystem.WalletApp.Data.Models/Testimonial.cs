namespace PaymentSystem.WalletApp.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using PaymentSystem.WalletApp.Data.Common.Models;

    public class Testimonial : BaseDeletableModel<string>
    {
        public Testimonial()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Required]
        [StringLength(5000)]
        public string Comment { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string UseCase { get; set; }
    }
}
