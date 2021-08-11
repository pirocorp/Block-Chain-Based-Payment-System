namespace PaymentSystem.WalletApp.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using PaymentSystem.WalletApp.Services.Data;
    using PaymentSystem.WalletApp.Web.ViewModels.Administration.Accounts;
    using PaymentSystem.WalletApp.Web.ViewModels.Administration.Blocks;
    using PaymentSystem.WalletApp.Web.ViewModels.Administration.Dashboard;
    using PaymentSystem.WalletApp.Web.ViewModels.Administration.Users;

    public class DashboardController : AdministrationController
    {
        private readonly IAccountService accountService;
        private readonly IBlockService blockService;
        private readonly IUserService userService;

        public DashboardController(
            IAccountService accountService,
            IBlockService blockService,
            IUserService userService)
        {
            this.accountService = accountService;
            this.blockService = blockService;
            this.userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new AdminDashboardIndexViewModel()
            {
                Accounts = await this.accountService.GetLatestAccounts<AccountListingAdminModel>(),
                Blocks = await this.blockService.GetLatestBlocks<BlockListingAdminModel>(),
                Users = await this.userService.GetLatestRegisteredUsers<UserListingAdminModel>(),
            };

            return this.View(model);
        }
    }
}
