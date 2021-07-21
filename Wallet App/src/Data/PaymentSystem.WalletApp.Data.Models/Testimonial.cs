namespace PaymentSystem.WalletApp.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using PaymentSystem.WalletApp.Data.Common.Models;

    using static Data.Common.DataConstants.Testimonial;

    public class Testimonial : BaseDeletableModel<string>
    {
        public Testimonial()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Required]
        [StringLength(CommentLength)]
        public string Comment { get; set; }

        [Required]
        [StringLength(NameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(UseCaseLength)]
        public string UseCase { get; set; }
    }
}
