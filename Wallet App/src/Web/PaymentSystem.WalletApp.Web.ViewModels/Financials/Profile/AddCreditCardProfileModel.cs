namespace PaymentSystem.WalletApp.Web.ViewModels.Financials.Profile
{
    using System.ComponentModel.DataAnnotations;

    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Web.Infrastructure.ValidationAttributes;

    public class AddCreditCardProfileModel
    {
        [Required]
        public bool IsCredit { get; set; }

        public CardType CardType { get; set; }

        [Required]
        [CreditCard]
        public string CardNumber { get; set; }

        [Required]
        [CreditCardExpiryDate]
        public string ExpiryDate { get; set; }

        [Required]
        [CVV]
        public string CVV { get; set; }

        [Required]
        public string CardHolderName { get; set; }
    }
}
