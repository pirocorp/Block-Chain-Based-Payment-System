namespace PaymentSystem.WalletApp.Web.ViewModels.Administration.Accounts
{
    using System.Collections.Generic;

    using PaymentSystem.WalletApp.Web.ViewModels.Administration.Transactions;

    public class AccountDetailsAdminViewModel
    {
        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public AccountDetailsModel Account { get; set; }

        public IEnumerable<TransactionListingAdminModel> Transactions { get; set; }
    }
}
