namespace PaymentSystem.WalletApp.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using PaymentSystem.WalletApp.Data.Common.Models;

    using static Data.Common.DataConstants.CreditCard;

    /// <summary>
    /// Don't do it in real project. Use payment processor instead.
    /// </summary>
    public class CreditCard : BaseModel<string>
    {
        public CreditCard()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public bool IsCredit { get; set; }

        public CardType CardType { get; set; }

        public string CardData { get; set; }

        [StringLength(LastFourDigitsLength)]
        public string LastFourDigits { get; set; }

        [StringLength(ExpiryDateLength)]
        public string ExpiryDate { get; set; }

        [StringLength(SecurityStampLength)]
        public string SecurityStamp { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}
