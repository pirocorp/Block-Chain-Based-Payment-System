namespace PaymentSystem.WalletApp.Web.ViewModels.Administration.Transactions
{
    using System.Collections.Generic;

    public class TransactionsListingAdminPartialViewModel
    {
        public string Title { get; set; }

        public IEnumerable<TransactionListingAdminModel> Transactions { get; set; }
    }
}
