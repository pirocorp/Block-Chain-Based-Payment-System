namespace PaymentSystem.WalletApp.Services.Data.Models.BankAccounts
{
    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Data.Models;

    public class BankAccountsSelectListModel : IMapFrom<BankAccount>
    {
        public string Id { get; set; }

        public string BankName { get; set; }

        public string IBAN { get; set; }
    }
}
