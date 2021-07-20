namespace PaymentSystem.WalletApp.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using PaymentSystem.WalletApp.Data.Common.Models;

    /// <summary>
    /// Don't do it in real project. Use payment processor instead.
    /// </summary>
    public class CreditCard : BaseDeletableModel<string>
    {
        public CreditCard()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public bool IsCredit { get; set; }

        public CardType CardType { get; set; }

        public string CardData { get; set; }

        [StringLength(4)]
        public string LastFourDigits { get; set; }

        [StringLength(5)]
        public string ExpiryDate { get; set; }

        [StringLength(128)]
        public string SecurityStamp { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}
