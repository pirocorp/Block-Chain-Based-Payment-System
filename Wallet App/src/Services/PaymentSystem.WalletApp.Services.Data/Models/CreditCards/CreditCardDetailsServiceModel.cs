namespace PaymentSystem.WalletApp.Services.Data.Models.CreditCards
{
    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data.Models;

    public class CreditCardDetailsServiceModel : IMapFrom<CreditCard>
    {
        public string Id { get; set; }

        public string LastFourDigits { get; set; }

        public string ExpiryDate { get; set; }

        public string CardType { get; set; }

        public string CVV { get; set; }

        public string CardHolderName { get; set; }
    }
}
