namespace PaymentSystem.WalletApp.Services.Data.Models
{
    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data.Models;

    public class AddCreditCardServiceModel
    {
        public bool IsCredit { get; set; }

        public CardType CardType { get; set; }

        public string CardNumber { get; set; }

        public string ExpiryDate { get; set; }

        public string CVV { get; set; }

        public string CardHolderName { get; set; }
    }
}
