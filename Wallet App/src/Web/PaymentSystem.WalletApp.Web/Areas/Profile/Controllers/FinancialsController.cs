namespace PaymentSystem.WalletApp.Web.Areas.Profile.Controllers
{
    using System.Text;
    using System.Threading.Tasks;

    using AutoMapper;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services;
    using PaymentSystem.WalletApp.Services.Data;
    using PaymentSystem.WalletApp.Services.Data.Models.BankAccounts;
    using PaymentSystem.WalletApp.Services.Data.Models.CreditCards;
    using PaymentSystem.WalletApp.Web.Infrastructure.Helpers;
    using PaymentSystem.WalletApp.Web.Infrastructure.Options;
    using PaymentSystem.WalletApp.Web.ViewModels.Financials.Profile;

    [Authorize]
    public class FinancialsController : ProfileController
    {
        private readonly ICreditCardService creditCardService;
        private readonly IBankAccountService bankAccountService;
        private readonly IFingerprintService fingerprintService;
        private readonly IOptions<EncryptionOptions> encryptionOptions;

        public FinancialsController(
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            ICloudinaryService cloudinaryService,
            IUserService userService,
            ICreditCardService creditCardService,
            IBankAccountService bankAccountService,
            IFingerprintService fingerprintService,
            IOptions<EncryptionOptions> encryptionOptions)
            : base(userManager, mapper, cloudinaryService, userService)
        {
            this.creditCardService = creditCardService;
            this.bankAccountService = bankAccountService;
            this.fingerprintService = fingerprintService;
            this.encryptionOptions = encryptionOptions;
        }

        public async Task<IActionResult> GetCreditCardDetails(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return this.BadRequest();
            }

            if (!await this.creditCardService.Exists(id))
            {
                return this.BadRequest();
            }

            var userId = this.UserManager.GetUserId(this.User);

            if (!await this.creditCardService.UserOwnsCard(id, userId))
            {
                return this.BadRequest();
            }

            var key = Encoding.UTF8.GetBytes(this.encryptionOptions.Value.Key);

            var model = await this.creditCardService.GetCardInformation(id, key);
            return this.Ok(model);
        }

        public async Task<IActionResult> GetBankAccountDetails(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return this.BadRequest();
            }

            if (!await this.bankAccountService.Exists(id))
            {
                return this.BadRequest();
            }

            var userId = this.UserManager.GetUserId(this.User);

            if (!await this.bankAccountService.UserOwnsAccount(id, userId))
            {
                return this.BadRequest();
            }

            var model = await this.bankAccountService.GetAccountInformation<ProfileDeleteBankAccountModel>(id);
            return this.Ok(model);
        }

        public async Task<IActionResult> Index()
        {
            var profileUser = await this.GetUserProfile();

            return this.View(profileUser);
        }

        [HttpPost]
        public async Task<IActionResult> AddCreditCard(AddCreditCardProfileModel model)
        {
            if (await this.fingerprintService.Exists(model.CardNumber))
            {
                this.ModelState.AddModelError(string.Empty, "Card number already exists.");
            }

            if (!this.ModelState.IsValid)
            {
                var profileUser = await this.GetUserProfile();
                return this.View(nameof(this.Index), profileUser);
            }

            var serviceModel = this.Mapper.Map<AddCreditCardServiceModel>(model);
            var userId = this.UserManager.GetUserId(this.User);
            var key = Encoding.UTF8.GetBytes(this.encryptionOptions.Value.Key);

            await this.creditCardService.AddCreditCard(serviceModel, userId, key);

            return this.RedirectToAction<ProfileController, FinancialsController>(nameof(this.Index));
        }

        [HttpPost]
        public async Task<IActionResult> EditCreditCard(EditCreditCardModel model)
        {
            var userId = this.UserManager.GetUserId(this.User);

            if (!await this.creditCardService.UserOwnsCard(model.Id, userId))
            {
                return this.BadRequest();
            }

            if (!this.ModelState.IsValid)
            {
                var profileUser = await this.GetUserProfile();
                return this.View(nameof(this.Index), profileUser);
            }

            var key = Encoding.UTF8.GetBytes(this.encryptionOptions.Value.Key);

            var serviceModel = this.Mapper.Map<EditCreditCardServiceModel>(model);
            await this.creditCardService.UpdateCardInformation(key, serviceModel);

            return this.RedirectToAction<ProfileController, FinancialsController>(nameof(this.Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCreditCard(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return this.BadRequest();
            }

            if (!await this.creditCardService.Exists(id))
            {
                return this.BadRequest();
            }

            var userId = this.UserManager.GetUserId(this.User);

            if (!await this.creditCardService.UserOwnsCard(id, userId))
            {
                return this.BadRequest();
            }

            await this.creditCardService.DeleteCreditCard(id);

            return this.RedirectToAction<ProfileController, FinancialsController>(nameof(this.Index));
        }

        [HttpPost]
        public async Task<IActionResult> AddBankAccount(AddBankAccountProfileModel model)
        {
            if (!this.ModelState.IsValid)
            {
                var profileUser = await this.GetUserProfile();
                return this.View(nameof(this.Index), profileUser);
            }

            var userId = this.UserManager.GetUserId(this.User);

            var serviceModel = this.Mapper.Map<AddBankAccountServiceModel>(model);
            await this.bankAccountService.AddAccount(serviceModel, userId);

            return this.RedirectToAction<ProfileController, FinancialsController>(nameof(this.Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBankAccount(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return this.BadRequest();
            }

            if (!await this.bankAccountService.Exists(id))
            {
                return this.BadRequest();
            }

            await this.bankAccountService.DeleteAccount(id);

            return this.RedirectToAction<ProfileController, FinancialsController>(nameof(this.Index));
        }

        private async Task<ProfileFinancialViewModel> GetUserProfile()
        {
            var profileUser = await this.GetProfileUser<ProfileFinancialViewModel>();

            profileUser.CreditCards = await this.creditCardService.GetCreditCards<ProfileCreditCardModel>(profileUser.Id);
            profileUser.BankAccounts = await this.bankAccountService.GetAccounts<ProfileBankAccountModel>(profileUser.Id);

            return profileUser;
        }
    }
}
