namespace PaymentSystem.WalletApp.Web.Areas.Profile.Controllers
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

        public UsersController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IMapper mapper,
            ICloudinaryService cloudinaryService,
            IUserService userService)
            : base(userManager, mapper, cloudinaryService, userService)
        {
            this.signInManager = signInManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = this.UserManager.GetUserId(this.User);
            var dashboardUser = await this.UserService.GetUser<DashboardUser>(userId);

            dashboardUser.ProfilePictureAddress =
                this.CloudinaryService.GetProfileImageAddress(dashboardUser.ProfilePicture);

            return this.View(dashboardUser);
        }

        public async Task<IActionResult> Index()
        {
            var profileUser = await this.GetProfileUser();

            return this.View(profileUser);
        }

        [HttpPost]
        public async Task<IActionResult> Index(PersonalDetailsUpdateModel model)
        {
            if (!this.ModelState.IsValid)
            {
                var profileUser = await this.GetProfileUser();
                return this.View(profileUser);
            }

            var user = await this.UserManager.GetUserAsync(this.User);

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.DateOfBirth = DateTime.ParseExact(model.BirthDate, WalletConstants.BirthDateFormat, CultureInfo.InvariantCulture);

            var address = new Address()
            {
                Street = model.Street,
                City = model.City,
                StateProvince = model.StateProvince,
                Zip = model.ZipCode,
                Country = model.Country,
            };

            user.Address = address;

            await this.UserManager.UpdateAsync(user);

            return this.RedirectToAction<ProfileController, UsersController>(nameof(this.Index));
        }

        [HttpPost]
        public async Task<IActionResult> EmailUpdate(EmailUpdateModel model)
        {
            if (!this.ModelState.IsValid)
            {
                var profileUser = await this.GetProfileUser();
                return this.View(nameof(this.Index), profileUser);
            }

            var user = await this.UserManager.GetUserAsync(this.User);

            var emailChangeToken = await this.UserManager.GenerateChangeEmailTokenAsync(user, model.Email);
            var identityResult = await this.UserManager.ChangeEmailAsync(user, model.Email, emailChangeToken);

            if (!identityResult.Succeeded)
            {
                foreach (var error in identityResult.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }

                var profileUser = await this.GetProfileUser();
                return this.View(nameof(this.Index), profileUser);
            }

            return this.RedirectToAction<ProfileController, UsersController>(nameof(this.Index));
        }

        [HttpPost]
        public async Task<IActionResult> PhoneUpdate(PhoneUpdateModel model)
        {
            if (!this.ModelState.IsValid)
            {
                var profileUser = await this.GetProfileUser();
                return this.View(nameof(this.Index), profileUser);
            }

            var user = await this.UserManager.GetUserAsync(this.User);
            user.PhoneNumber = model.Phone;
            await this.UserManager.UpdateAsync(user);

            return this.RedirectToAction<ProfileController, UsersController>(nameof(this.Index));
        }

        [HttpPost]
        public async Task<IActionResult> UploadPicture([FromForm] IFormFile profilePicture)
        {
            var controller = ControllerHelpers.GetControllerName<UsersController>();

            if (!this.ModelState.IsValid)
            {
                var profileUser = await this.GetProfileUser();
                return this.View(nameof(this.Index), profileUser);
            }

            var profilePictureAddress = await this.CloudinaryService.Upload(profilePicture);

            var user = await this.UserManager.GetUserAsync(this.User);
            user.ProfilePicture = profilePictureAddress;
            await this.UserManager.UpdateAsync(user);

            return this.RedirectToAction<ProfileController, UsersController>(nameof(this.Index));
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!this.ModelState.IsValid)
            {
                var profileUser = await this.GetProfileUser();
                return this.View(nameof(this.Index), profileUser);
            }

            var user = await this.UserManager.GetUserAsync(this.User);

            var result = await this.signInManager.UserManager
                .ChangePasswordAsync(user, model.Password, model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }

                var profileUser = await this.GetProfileUser();
                return this.View(nameof(this.Index), profileUser);
            }

            return this.RedirectToAction<ProfileController, UsersController>(nameof(this.Index));
        }

        private async Task<ProfileUserViewModel> GetProfileUser()
        {
            var profileUser = await this.GetProfileUser<ProfileUserViewModel>();

            profileUser.Address ??= new Address();

            return profileUser;
        }
    }
}
