namespace PaymentSystem.WalletApp.Web.Controllers
{
    using System.Threading.Tasks;

    using AutoMapper;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;

    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services;
    using PaymentSystem.WalletApp.Web.ViewModels.ProfileLayout;
    using Services.Data;

    [Authorize]
    public abstract class ProfileController : BaseController
    {
        protected readonly UserManager<ApplicationUser> userManager;
        protected readonly IMapper mapper;
        protected readonly ICloudinaryService cloudinaryService;
        protected readonly IUserService userService;

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

        protected async Task<T> GetProfileUser<T>()
            where T : ProfileLayoutUserModel
        {
            var userId = this.userManager.GetUserId(this.User);
            var profileUser = await this.userService.GetUser<T>(userId);

            profileUser.ProfilePictureAddress =
                this.cloudinaryService.GetProfileImageAddress(profileUser.ProfilePicture);

            return profileUser;
        }
    }
}
