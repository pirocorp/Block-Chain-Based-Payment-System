namespace PaymentSystem.WalletApp.Web.ViewModels.Accounts.Transactions
{
    using System.Collections.Generic;

    public class AccountsTransactionsViewModel
    {
        public AccountsTransactionsViewModel()
        {
            this.Transactions = new List<AccountsTransactionsListingModel>();
        }

        public IEnumerable<AccountsTransactionsListingModel> Transactions { get; set; }

        public string CurrentAccountAddress { get; set; }

        public int TotalPages { get; set; }

        public int CurrentPage { get; set; }
    }
}
