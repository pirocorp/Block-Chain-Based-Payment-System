namespace PaymentSystem.WalletApp.Web.ViewModels.Accounts.Profile
{
    using System.Collections.Generic;

    using PaymentSystem.WalletApp.Web.ViewModels.ProfileLayout;

    public class ProfileAccountsViewModel : ProfileLayoutUserModel
    {
        public ProfileAccountsViewModel()
        {
            this.Accounts = new List<ProfileAccountModel>();
        }

        public IEnumerable<ProfileAccountModel> Accounts { get; set; }
    }
}
