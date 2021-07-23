namespace PaymentSystem.WalletApp.Web.Controllers
{
    using System.Threading.Tasks;

    using AutoMapper;
    using Infrastructure.Helpers;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using PaymentSystem.WalletApp.Data.Models;

    using PaymentSystem.WalletApp.Services;
    using PaymentSystem.WalletApp.Web.ViewModels.Accounts.Profile;

    public class AccountsController : ProfileController
    {
        public AccountsController(
            UserManager<ApplicationUser> userManager, 
            IMapper mapper, 
            ICloudinaryService cloudinaryService) 
            : base(userManager, mapper, cloudinaryService)
        {
        }

        public async Task<IActionResult> Profile()
        {
            var profileUser = await this.GetUserProfile();

            return this.View(profileUser);
        }

        public async Task<IActionResult> CreateCoinAccount()
        {
            var controller = ControllerHelpers.GetControllerName<AccountsController>();
            return this.Redirect($"/{controller}/{nameof(this.Profile)}");
        }

        private async Task<ProfileAccountsViewModel> GetUserProfile()
        {
            var profileUser = await this.GetProfileUser<ProfileAccountsViewModel>();

            return profileUser;
        }
    }
}
