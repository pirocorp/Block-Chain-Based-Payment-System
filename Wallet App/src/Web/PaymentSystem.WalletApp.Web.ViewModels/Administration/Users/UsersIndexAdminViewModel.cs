namespace PaymentSystem.WalletApp.Web.ViewModels.Administration.Users
{
    using System.Collections.Generic;

    public class UsersIndexAdminViewModel
    {
        public int TotalPages { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<UserListingAdminModel> Users { get; set; }
    }
}
