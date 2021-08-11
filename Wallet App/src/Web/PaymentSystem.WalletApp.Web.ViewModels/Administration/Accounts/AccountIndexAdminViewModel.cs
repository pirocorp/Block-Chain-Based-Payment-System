namespace PaymentSystem.WalletApp.Web.ViewModels.Administration.Accounts
{
    using System.Collections.Generic;

    public class AccountIndexAdminViewModel
    {
        public int TotalPages { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<AccountListingAdminModel> Accounts { get; set; }
    }
}
