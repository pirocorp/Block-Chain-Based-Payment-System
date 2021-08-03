namespace PaymentSystem.WalletApp.Web.Controllers
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;

    using AutoMapper;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using PaymentSystem.WalletApp.Common;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services;
    using PaymentSystem.WalletApp.Services.Data;
    using PaymentSystem.WalletApp.Web.Infrastructure.Helpers;
    using PaymentSystem.WalletApp.Web.ViewModels.Users.Dashboard;
    using PaymentSystem.WalletApp.Web.ViewModels.Users.Profile;

    [Authorize]
    public class UsersController : ProfileController
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ITransactionService transactionService;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IMapper mapper,
            ICloudinaryService cloudinaryService,
            IUserService userService,
            ITransactionService transactionService) 
            : base(userManager, mapper, cloudinaryService, userService)
        {
            this.signInManager = signInManager;
            this.transactionService = transactionService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = this.userManager.GetUserId(this.User);
            var dashboardUser = await this.userService.GetUser<DashboardUser>(userId);

            dashboardUser.ProfilePictureAddress =
                this.cloudinaryService.GetProfileImageAddress(dashboardUser.ProfilePicture);

            return this.View(dashboardUser);
        }

        public async Task<IActionResult> Profile()
        {
            var profileUser = await this.GetProfileUser();

            return this.View(profileUser);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(PersonalDetailsUpdateModel model)
        {
            if (!this.ModelState.IsValid)
            {
                var profileUser = await this.GetProfileUser();
                return this.View(profileUser);
            }

            var user = await this.userManager.GetUserAsync(this.User);

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.DateOfBirth = DateTime.ParseExact(model.BirthDate, WalletConstants.BirthDateFormat, CultureInfo.InvariantCulture);

            var address = new Address()
            {
                Street = model.Street,
                City = model.City,
                StateProvince = model.StateProvince,
                Zip = model.ZipCode,
                Country = model.Country
            };

            user.Address = address;

            await this.userManager.UpdateAsync(user);

            var controller = ControllerHelpers.GetControllerName<UsersController>();
            return this.Redirect($"/{controller}/{nameof(this.Profile)}");
        }

        [HttpPost]
        public async Task<IActionResult> EmailUpdate(EmailUpdateModel model)
        {
            if (!this.ModelState.IsValid)
            {
                var profileUser = await this.GetProfileUser();
                return this.View(nameof(this.Profile), profileUser);
            }

            var user = await this.userManager.GetUserAsync(this.User);
            user.Email = model.Email;
            await this.userManager.UpdateAsync(user);

            var controller = ControllerHelpers.GetControllerName<UsersController>();
            return this.Redirect($"/{controller}/{nameof(this.Profile)}");
        }

        [HttpPost]
        public async Task<IActionResult> PhoneUpdate(PhoneUpdateModel model)
        {
            var controller = ControllerHelpers.GetControllerName<UsersController>();

            if (!this.ModelState.IsValid)
            {
                var profileUser = await this.GetProfileUser();
                return this.View(nameof(this.Profile), profileUser);
            }

            var user = await this.userManager.GetUserAsync(this.User);
            user.PhoneNumber = model.Phone;
            await this.userManager.UpdateAsync(user);

            return this.Redirect($"/{controller}/{nameof(this.Profile)}");
        }

        [HttpPost]
        public async Task<IActionResult> UploadPicture([FromForm] IFormFile profilePicture)
        {
            var controller = ControllerHelpers.GetControllerName<UsersController>();

            if (!this.ModelState.IsValid)
            {
                var profileUser = await this.GetProfileUser();
                return this.View(nameof(this.Profile), profileUser);
            }

            var profilePictureAddress = await this.cloudinaryService.Upload(profilePicture);

            var user = await this.userManager.GetUserAsync(this.User);
            user.ProfilePicture = profilePictureAddress;
            await this.userManager.UpdateAsync(user);

            return this.Redirect($"/{controller}/{nameof(this.Profile)}");
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            var controller = ControllerHelpers.GetControllerName<UsersController>();

            if (!this.ModelState.IsValid)
            {
                var profileUser = await this.GetProfileUser();
                return this.View(nameof(this.Profile), profileUser);
            }

            var user = await this.userManager.GetUserAsync(this.User);

            var result = await this.signInManager.UserManager
                .ChangePasswordAsync(user, model.Password, model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }

                var profileUser = await this.GetProfileUser();
                return this.View(nameof(this.Profile), profileUser);
            }

            return this.Redirect($"/{controller}/{nameof(this.Profile)}");
        }

        public async Task<IActionResult> GetTransactionDetails(string id)
        {
            var activity = await this.transactionService
                .GetTransaction<DashboardTransactionDetails>(id);

            return this.Ok(activity);
        }

        private async Task<ProfileUserViewModel> GetProfileUser()
        {
            var profileUser = await this.GetProfileUser<ProfileUserViewModel>();

            profileUser.Address ??= new Address();

            return profileUser;
        }
    }
}
