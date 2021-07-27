namespace PaymentSystem.WalletApp.Web.ViewModels.Transfers
{
    using System.ComponentModel.DataAnnotations;

    public enum DepositModelPaymentMethod
    {
        [Display(Name = "Credit or Debit card")]
        CreditOrDebitCard = 1,

        [Display(Name = "Bank Account")]
        BankAccount = 2,
    }
}
