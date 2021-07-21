namespace PaymentSystem.WalletApp.Services.Data.Models.CreditCards
{
    public class EditCreditCardServiceModel
    {
        public string Id { get; set; }

        public string ExpiryDate { get; set; }

        public string CVV { get; set; }

        public string CardHolderName { get; set; }
    }
}
