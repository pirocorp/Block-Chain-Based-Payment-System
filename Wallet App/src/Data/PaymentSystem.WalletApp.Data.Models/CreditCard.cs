namespace PaymentSystem.WalletApp.Data.Models
{
    using PaymentSystem.WalletApp.Data.Common.Models;

    public class CreditCard : BaseDeletableModel<string>
    {
        public bool IsCredit { get; set; }

        public CardType CardType { get; set; }

        public string CardNumber { get; set; }

        public string ExpiryDate { get; set; }

        public string CVV { get; set; }

        public string CardHolderName { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}
