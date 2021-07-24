namespace PaymentSystem.WalletApp.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using AutoMapper;
    using Infrastructure.Helpers;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using PaymentSystem.Common.GrpcService;
    using PaymentSystem.WalletApp.Data.Models;

    using PaymentSystem.WalletApp.Services;
    using PaymentSystem.WalletApp.Web.ViewModels.Accounts.Profile;
    using Services.Data;
    using Services.Data.Models.AccountsKeys;

    public class AccountsController : ProfileController
    {
        private const string AccountTempDataKey = "newAccountDetails";
        private readonly IAccountService accountService;
        private readonly IAccountsKeyService accountsKeyService;

        public AccountsController(
            UserManager<ApplicationUser> userManager, 
            IMapper mapper, 
            ICloudinaryService cloudinaryService,
            IAccountService accountService,
            IAccountsKeyService accountsKeyService) 
            : base(userManager, mapper, cloudinaryService)
        {
            this.accountService = accountService;
            this.accountsKeyService = accountsKeyService;
        }

        public async Task<IActionResult> Profile()
        {
            var profileUser = await this.GetUserProfile();

            return this.View(profileUser);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCoinAccount()
        {
            var userId = this.userManager.GetUserId(this.User);
            var accountData = await this.accountService.Create(userId);

            this.TempData[AccountTempDataKey] = JsonConvert.SerializeObject(accountData);

            var controller = ControllerHelpers.GetControllerName<AccountsController>();
            return this.Redirect($"/{controller}/{nameof(this.NewAccountDetails)}");
        }

        public async Task<IActionResult> NewAccountDetails()
        {
            var serializedObject = (string)this.TempData[AccountTempDataKey];

            if (string.IsNullOrWhiteSpace(serializedObject))
            {
                var controller = ControllerHelpers.GetControllerName<AccountsController>();
                return this.Redirect($"/{controller}/{nameof(this.Profile)}");
            }

            var accountData = JsonConvert.DeserializeObject<AccountCreationResponse>(serializedObject);

            var model = await this.GetProfileUser<NewAccountDetailsViewModel>();
            model.NewAccountDetails = accountData;

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> StoreSecret(StoreSecretModel model)
        {
            var userId = this.userManager.GetUserId(this.User);

            if (!await this.accountService.UserOwnsAccount(userId, model.Address))
            {
                return this.BadRequest();
            }

            var serviceModel = this.mapper.Map<StoreAccountKeyServiceModel>(model);
            await this.accountsKeyService.StoreKeys(serviceModel, userId);

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
