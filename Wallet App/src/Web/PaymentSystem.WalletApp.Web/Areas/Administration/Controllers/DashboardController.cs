namespace PaymentSystem.WalletApp.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using PaymentSystem.WalletApp.Services.Data;
    using PaymentSystem.WalletApp.Web.ViewModels.Administration.Dashboard;
    using PaymentSystem.WalletApp.Web.ViewModels.Administration.Users;
    using ViewModels.Administration.Accounts;

    public class DashboardController : AdministrationController
    {
        private readonly IAccountService accountService;
        private readonly IUserService userService;

        public DashboardController(
            IAccountService accountService,
            IUserService userService)
        {
            this.accountService = accountService;
            this.userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new AdminDashboardIndexViewModel()
            {
                Accounts = await this.accountService.GetLatestAccounts<AccountListingAdminModel>(),
                Users = await this.userService.GetLatestRegisteredUsers<UserListingAdminModel>(),
            };

            return this.View(model);
        }
    }
}
