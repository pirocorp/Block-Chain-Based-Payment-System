namespace PaymentSystem.WalletApp.Web.ViewModels.Administration.Accounts
{
    using System.Collections.Generic;

    public class AccountListingAdminPartialModel
    {
        public string Title { get; set; }

        public IEnumerable<AccountListingAdminModel> Accounts { get; set; }
    }
}
