﻿namespace PaymentSystem.WalletApp.Web.ViewModels.Administration.Dashboard
{
    using System.Collections.Generic;

    using PaymentSystem.WalletApp.Web.ViewModels.Administration.Accounts;
    using PaymentSystem.WalletApp.Web.ViewModels.Administration.Users;

    public class AdminDashboardIndexViewModel
    {
        public AdminDashboardIndexViewModel()
        {
            this.Accounts = new List<AccountListingAdminModel>();
            this.Users = new List<UserListingAdminModel>();
        }

        public IEnumerable<AccountListingAdminModel> Accounts { get; set; }

        public IEnumerable<UserListingAdminModel> Users { get; set; }
    }
}
