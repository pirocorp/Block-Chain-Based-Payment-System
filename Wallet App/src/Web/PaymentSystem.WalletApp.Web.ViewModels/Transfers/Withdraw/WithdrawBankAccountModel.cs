namespace PaymentSystem.WalletApp.Web.ViewModels.Transfers.Withdraw
{
    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data.Models;

    public class WithdrawBankAccountModel : IMapFrom<BankAccount>
    {
        public string Id { get; set; }

        public string BankName { get; set; }

        public string IBAN { get; set; }
    }
}
