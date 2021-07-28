namespace PaymentSystem.WalletApp.Services.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public enum PaymentMethod
    {
        [Display(Name = "Credit or Debit card")]
        CreditOrDebitCard = 1,

        [Display(Name = "Bank Account")]
        BankAccount = 2,
    }
}
