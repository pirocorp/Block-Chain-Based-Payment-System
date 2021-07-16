namespace PaymentSystem.WalletApp.Web.Controllers
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;

    using AutoMapper;

    using PaymentSystem.WalletApp.Common;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Web.Infrastructure.Helpers;
    using PaymentSystem.WalletApp.Web.ViewModels.Users.Profile;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Services;

    [Authorize]
    public class UsersController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IMapper mapper;
        private readonly ICloudinaryService cloudinaryService;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IMapper mapper,
            ICloudinaryService cloudinaryService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.mapper = mapper;
            this.cloudinaryService = cloudinaryService;
        }

        public async Task<IActionResult> Dashboard()
        {
            return this.View();
        }

        public async Task<IActionResult> Profile()
        {
            var user = await this.userManager.GetUserAsync(this.User);
            var profileUser = this.mapper.Map<ProfileUserViewModel>(user);

            profileUser.Address ??= new Address();
            profileUser.ProfilePictureAddress =
                this.cloudinaryService.GetProfileImageAddress(profileUser.ProfilePicture);

            return this.View(profileUser);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(PersonalDetailsUpdateModel model)
        {
            var controller = ControllerHelpers.GetControllerName<UsersController>();

            if (!this.ModelState.IsValid)
            {
                return this.Redirect($"/{controller}/{nameof(Profile)}");
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

            return this.Redirect($"/{controller}/{nameof(Profile)}");
        }

        [HttpPost]
        public async Task<IActionResult> EmailUpdate(EmailUpdateModel model)
        {
            var controller = ControllerHelpers.GetControllerName<UsersController>();

            if (!this.ModelState.IsValid)
            {
                return this.Redirect($"/{controller}/{nameof(Profile)}");
            }

            var user = await this.userManager.GetUserAsync(this.User);
            user.Email = model.Email;
            await this.userManager.UpdateAsync(user);

            return this.Redirect($"/{controller}/{nameof(Profile)}");
        }

        [HttpPost]
        public async Task<IActionResult> PhoneUpdate(PhoneUpdateModel model)
        {
            var controller = ControllerHelpers.GetControllerName<UsersController>();

            if (!this.ModelState.IsValid)
            {
                return this.Redirect($"/{controller}/{nameof(Profile)}");
            }

            var user = await this.userManager.GetUserAsync(this.User);
            user.PhoneNumber = model.Phone;
            await this.userManager.UpdateAsync(user);

            return this.Redirect($"/{controller}/{nameof(Profile)}");
        }

        [HttpPost]
        public async Task<IActionResult> UploadPicture([FromForm] IFormFile profilePicture)
        {
            var controller = ControllerHelpers.GetControllerName<UsersController>();

            if (!this.ModelState.IsValid)
            {
                return this.Redirect($"/{controller}/{nameof(Profile)}");
            }

            var profilePictureAddress = await this.cloudinaryService.Upload(profilePicture);

            var user = await this.userManager.GetUserAsync(this.User);
            user.ProfilePicture = profilePictureAddress;
            await this.userManager.UpdateAsync(user);

            return this.Redirect($"/{controller}/{nameof(Profile)}");
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            var controller = ControllerHelpers.GetControllerName<UsersController>();
            var newPasswordAndConfirmPasswordMatch = model.NewPassword == model.ConfirmPassword;

            if (!this.ModelState.IsValid || !newPasswordAndConfirmPasswordMatch)
            {
                return this.Redirect($"/{controller}/{nameof(Profile)}");
            }

            var user = await this.userManager.GetUserAsync(this.User);

            var result = await this.signInManager.UserManager
                .ChangePasswordAsync(user, model.Password, model.NewPassword);

            return this.Redirect($"/{controller}/{nameof(Profile)}");
        }
    }
}
