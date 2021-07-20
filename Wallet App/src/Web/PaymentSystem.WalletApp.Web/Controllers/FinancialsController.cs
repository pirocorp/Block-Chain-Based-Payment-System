namespace PaymentSystem.WalletApp.Web.Controllers
{
    using System.Threading.Tasks;
    using AutoMapper;
    using Data.Models;
    using Infrastructure.Helpers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Services;
    using ViewModels.Financials.Profile;
    using ViewModels.Users.Profile;

    [Authorize]
    public class FinancialsController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;
        private readonly ICloudinaryService cloudinaryService;

        public FinancialsController(
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            ICloudinaryService cloudinaryService)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.cloudinaryService = cloudinaryService;
        }

        public async Task<IActionResult> Profile()
        {
            var profileUser = await this.GetUserProfile();

            return this.View(profileUser);
        }

        [HttpPost]
        public async Task<IActionResult> AddCreditCard(AddCreditCardProfileModel model)
        {
            var controller = ControllerHelpers.GetControllerName<UsersController>();

            if (!this.ModelState.IsValid)
            {
                var profileUser = await this.GetUserProfile();
                return this.View(nameof(this.Profile), profileUser);
            }

            return this.Redirect($"/{controller}/{nameof(this.Profile)}");
        }

        private async Task<ProfileFinancialViewModel> GetUserProfile()
        {
            var user = await this.userManager.GetUserAsync(this.User);
            var profileUser = this.mapper.Map<ProfileFinancialViewModel>(user);

            profileUser.ProfilePictureAddress =
                this.cloudinaryService.GetProfileImageAddress(profileUser.ProfilePicture);
            return profileUser;
        }
    }
}
