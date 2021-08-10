namespace PaymentSystem.WalletApp.Web.ViewModels.Administration.Users
{
    using System.Collections.Generic;

    public class UsersListingAdminPartialModel
    {
        public string Title { get; set; }

        public IEnumerable<UserListingAdminModel> Users { get; set; }
    }
}
