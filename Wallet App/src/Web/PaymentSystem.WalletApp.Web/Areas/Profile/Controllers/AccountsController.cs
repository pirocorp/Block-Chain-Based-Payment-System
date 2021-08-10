namespace PaymentSystem.WalletApp.Web.Areas.Profile.Controllers
{
    using System.Threading.Tasks;

    using AutoMapper;
    using Infrastructure.Filters.ActionFilters;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using Newtonsoft.Json;

    using PaymentSystem.Common.GrpcService;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services;
    using PaymentSystem.WalletApp.Services.Data;
    using PaymentSystem.WalletApp.Services.Data.Models.Accounts;
    using PaymentSystem.WalletApp.Services.Data.Models.AccountsKeys;
    using PaymentSystem.WalletApp.Web.ViewModels.Profile.Accounts.Profile;

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
            IAccountsKeyService accountsKeyService,
            IUserService userService)
            : base(userManager, mapper, cloudinaryService, userService)
        {
            this.accountService = accountService;
            this.accountsKeyService = accountsKeyService;
        }

        /// <summary>
        /// This endpoint is used by site javascript.
        /// </summary>
        /// <param name="address">Account address.</param>
        /// <returns>Serialized account information in JSON.</returns>
        public async Task<IActionResult> GetAccountDetails(string address)
            => this.Ok(await this.accountService.GetAccount<ProfileAccountModel>(address));

        public async Task<IActionResult> Index()
        {
            var profileUser = await this.GetUserProfile();

            return this.View(profileUser);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCoinAccount()
        {
            var userId = this.UserManager.GetUserId(this.User);
            var accountData = await this.accountService.Create(userId);

            this.TempData[AccountTempDataKey] = JsonConvert.SerializeObject(accountData);

            return this.RedirectToAction<ProfileController, AccountsController>(nameof(this.NewAccountDetails));
        }

        public async Task<IActionResult> NewAccountDetails()
        {
            if (!this.TempData.ContainsKey(AccountTempDataKey))
            {
                return this.BadRequest();
            }

            var serializedObject = (string)this.TempData[AccountTempDataKey];

            if (string.IsNullOrWhiteSpace(serializedObject))
            {
                return this.RedirectToAction<ProfileController, AccountsController>(nameof(this.Index));
            }

            var accountData = JsonConvert.DeserializeObject<AccountCreationResponse>(serializedObject);

            var model = await this.GetProfileUser<NewAccountDetailsViewModel>();
            model.NewAccountDetails = accountData;

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> StoreSecret(StoreSecretModel model)
        {
            var userId = this.UserManager.GetUserId(this.User);

            if (!await this.accountService.UserOwnsAccount(model.Address, userId))
            {
                return this.BadRequest();
            }

            var serviceModel = this.Mapper.Map<StoreAccountKeyServiceModel>(model);
            await this.accountsKeyService.StoreKeys(serviceModel, userId);

            return this.RedirectToAction<ProfileController, AccountsController>(nameof(this.Index));
        }

        [HttpPost]
        public async Task<IActionResult> EditAccountDetails(EditAccountDetailsModel model)
        {
            var userId = this.UserManager.GetUserId(this.User);

            if (!await this.accountService.UserOwnsAccount(model.Address, userId))
            {
                return this.BadRequest();
            }

            var serviceModel = this.Mapper.Map<EditAccountServiceModel>(model);
            await this.accountService.EditAccount(serviceModel);

            return this.RedirectToAction<ProfileController, AccountsController>(nameof(this.Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCoinAccount(string address)
        {
            var userId = this.UserManager.GetUserId(this.User);

            if (!await this.accountService.UserOwnsAccount(address, userId))
            {
                return this.BadRequest();
            }

            await this.accountService.Delete(address);

            return this.RedirectToAction<ProfileController, AccountsController>(nameof(this.Index));
        }

        private async Task<ProfileAccountsViewModel> GetUserProfile()
        {
            var profileUser = await this.GetProfileUser<ProfileAccountsViewModel>();

            var userId = this.UserManager.GetUserId(this.User);
            profileUser.Accounts = await this.accountService.GetUserAccounts<ProfileAccountModel>(userId);

            return profileUser;
        }
    }
}
