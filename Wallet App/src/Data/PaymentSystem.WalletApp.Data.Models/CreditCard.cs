namespace PaymentSystem.WalletApp.Data.Models
{
    using System;

    using PaymentSystem.WalletApp.Data.Common.Models;

    /// <summary>
    /// Don't do it in real project. Use payment processor instead.
    /// </summary>
    public class CreditCard : BaseDeletableModel<string>
    {
        public bool IsCredit { get; set; }

        public CardType CardType { get; set; }

        public string CardData { get; set; }

        public string SecurityStamp { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}
