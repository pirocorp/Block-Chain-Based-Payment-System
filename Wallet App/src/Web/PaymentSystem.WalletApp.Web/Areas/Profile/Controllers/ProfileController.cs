namespace PaymentSystem.WalletApp.Web.Areas.Profile.Controllers
{
    using System.Threading.Tasks;

    using AutoMapper;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services;
    using PaymentSystem.WalletApp.Services.Data;
    using PaymentSystem.WalletApp.Web.Controllers;
    using PaymentSystem.WalletApp.Web.ViewModels.ProfileLayout;

    [Authorize]
    [Area("Profile")]
    public abstract class ProfileController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;
        private readonly ICloudinaryService cloudinaryService;
        private readonly IUserService userService;

        protected ProfileController(
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            ICloudinaryService cloudinaryService,
            IUserService userService)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.cloudinaryService = cloudinaryService;
            this.userService = userService;
        }

        protected UserManager<ApplicationUser> UserManager => this.userManager;

        protected IMapper Mapper => this.mapper;

        protected ICloudinaryService CloudinaryService => this.cloudinaryService;

        protected IUserService UserService => this.userService;

        protected async Task<T> GetProfileUser<T>()
            where T : ProfileLayoutUserModel
        {
            var userId = this.UserManager.GetUserId(this.User);
            var profileUser = await this.UserService.GetUser<T>(userId);

            profileUser.ProfilePictureAddress =
                this.CloudinaryService.GetProfileImageAddress(profileUser.ProfilePicture);

            return profileUser;
        }
    }
}
