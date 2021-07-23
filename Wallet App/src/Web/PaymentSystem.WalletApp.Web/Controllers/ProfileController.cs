namespace PaymentSystem.WalletApp.Web.Controllers
{
    using System.Threading.Tasks;

    using AutoMapper;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;

    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services;
    using PaymentSystem.WalletApp.Web.ViewModels.ProfileLayout;

    [Authorize]
    public abstract class ProfileController : BaseController
    {
        protected readonly UserManager<ApplicationUser> userManager;
        protected readonly IMapper mapper;
        protected readonly ICloudinaryService cloudinaryService;

        protected ProfileController(
            UserManager<ApplicationUser> userManager, 
            IMapper mapper, 
            ICloudinaryService cloudinaryService)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.cloudinaryService = cloudinaryService;
        }

        protected async Task<T> GetProfileUser<T>()
            where T : ProfileLayoutUserModel
        {
            var user = await this.userManager.GetUserAsync(this.User);
            var profileUser = this.mapper.Map<T>(user);

            profileUser.ProfilePictureAddress =
                this.cloudinaryService.GetProfileImageAddress(profileUser.ProfilePicture);

            return profileUser;
        }
    }
}
