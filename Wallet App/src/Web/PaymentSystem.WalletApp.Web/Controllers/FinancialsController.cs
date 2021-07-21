namespace PaymentSystem.WalletApp.Web.Controllers
{
    using System.Text;
    using System.Threading.Tasks;
    using AutoMapper;
    using Data.Models;
    using Infrastructure;
    using Infrastructure.Helpers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Services;
    using Services.Data;
    using Services.Data.Models.CreditCards;
    using ViewModels.Financials.Profile;

    [Authorize]
    public class FinancialsController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;
        private readonly ICloudinaryService cloudinaryService;
        private readonly ICreditCardService creditCardService;
        private readonly IOptions<EncryptionOptions> encryptionOptions;

        public FinancialsController(
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            ICloudinaryService cloudinaryService,
            ICreditCardService creditCardService,
            IOptions<EncryptionOptions> encryptionOptions)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.cloudinaryService = cloudinaryService;
            this.creditCardService = creditCardService;
            this.encryptionOptions = encryptionOptions;
        }

        public async Task<IActionResult> Profile()
        {
            var profileUser = await this.GetUserProfile();

            return this.View(profileUser);
        }

        [HttpPost]
        public async Task<IActionResult> AddCreditCard(AddCreditCardProfileModel model)
        {
            if (!this.ModelState.IsValid)
            {
                var profileUser = await this.GetUserProfile();
                return this.View(nameof(this.Profile), profileUser);
            }

            var controller = ControllerHelpers.GetControllerName<UsersController>();

            var serviceModel = this.mapper.Map<AddCreditCardServiceModel>(model);
            var userId = this.userManager.GetUserId(this.User);
            var key = Encoding.UTF8.GetBytes(this.encryptionOptions.Value.Key);

            await this.creditCardService.AddCreditCard(serviceModel, userId, key);

            return this.Redirect($"/{controller}/{nameof(this.Profile)}");
        }

        [HttpPost]
        public async Task<IActionResult> EditCreditCard(EditCreditCardModel model)
        {
            var userId = this.userManager.GetUserId(this.User);

            if (!await this.creditCardService.UserOwnsCard(model.Id, userId))
            {
                return this.BadRequest();
            }

            if (!this.ModelState.IsValid)
            {
                var profileUser = await this.GetUserProfile();
                return this.View(nameof(this.Profile), profileUser);
            }

            var key = Encoding.UTF8.GetBytes(this.encryptionOptions.Value.Key);

            var serviceModel = this.mapper.Map<EditCreditCardServiceModel>(model);
            await this.creditCardService.UpdateCardInformation(key, serviceModel);

            var controller = ControllerHelpers.GetControllerName<UsersController>();

            return this.Redirect($"/{controller}/{nameof(this.Profile)}");
        }

        public async Task<IActionResult> GetCreditCardDetails(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return this.BadRequest();
            }

            var userId = this.userManager.GetUserId(this.User);

            if (!await this.creditCardService.UserOwnsCard(id, userId))
            {
                return this.BadRequest();
            }

            var key = Encoding.UTF8.GetBytes(this.encryptionOptions.Value.Key);

            var model = await this.creditCardService
                .GetCardInformation(id, key);

            return this.Ok(model);
        }

        private async Task<ProfileFinancialViewModel> GetUserProfile()
        {
            var user = await this.userManager.GetUserAsync(this.User);
            var profileUser = this.mapper.Map<ProfileFinancialViewModel>(user);

            profileUser.ProfilePictureAddress =
                this.cloudinaryService.GetProfileImageAddress(profileUser.ProfilePicture);

            profileUser.CreditCards = await this.creditCardService.GetCreditCards<ProfileCreditCardModel>(user.Id);
            
            return profileUser;
        }
    }
}
