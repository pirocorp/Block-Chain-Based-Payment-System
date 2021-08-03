namespace PaymentSystem.WalletApp.Web.Controllers
{
    using System.Threading.Tasks;

    using AutoMapper;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using Newtonsoft.Json;

    using PaymentSystem.Common;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services.Data;
    using PaymentSystem.WalletApp.Services.Data.Models;
    using PaymentSystem.WalletApp.Services.Data.Models.Transactions;
    using PaymentSystem.WalletApp.Web.Infrastructure;
    using PaymentSystem.WalletApp.Web.Infrastructure.Helpers;
    using PaymentSystem.WalletApp.Web.ViewModels.Transfers.Deposit;
    using PaymentSystem.WalletApp.Web.ViewModels.Transfers.DepositConfirm;
    using PaymentSystem.WalletApp.Web.ViewModels.Transfers.Withdraw;

    [Authorize]
    public class TransfersController : BaseController
    {
        private const string DepositTempData = "depositTempData";

        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;
        private readonly IAccountService accountService;
        private readonly IBankAccountService bankAccountService;
        private readonly ICreditCardService creditCardService;
        private readonly ITransferService transferService;
        private readonly IUserService userService;

        public TransfersController(
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IAccountService accountService,
            IBankAccountService bankAccountService,
            ICreditCardService creditCardService,
            ITransferService transferService,
            IUserService userService)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.accountService = accountService;
            this.bankAccountService = bankAccountService;
            this.creditCardService = creditCardService;
            this.transferService = transferService;
            this.userService = userService;
        }

        public IActionResult Deposit()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult Deposit(DepositModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this.TempData[DepositTempData] = JsonConvert.SerializeObject(model);

            return this.Redirect($"/{this.ControllerName}/{nameof(this.DepositConfirm)}");
        }

        public async Task<IActionResult> DepositConfirm()
        {
            if (!this.TempData.ContainsKey(DepositTempData))
            {
                return this.View(nameof(this.Deposit));
            }

            var serializedObject = (string)this.TempData[DepositTempData];
            var depositModel = JsonConvert.DeserializeObject<DepositModel>(serializedObject);
            var userId = this.userManager.GetUserId(this.User);

            var model = new DepositConfirmModel()
            {
                Amount = depositModel.Amount,
                Total = depositModel.Amount,
                PaymentMethod = depositModel.PaymentMethod,
                PaymentMethods = await this.userService.GetPaymentMethods(userId, depositModel.PaymentMethod),
                Accounts = await this.userService.GetCoinAccounts(userId),
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DepositConfirm(DepositConfirmInputModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(nameof(this.Deposit));
            }

            var userId = this.userManager.GetUserId(this.User);
            var userOwnsAccount = await this.accountService.UserOwnsAccount(model.Account, userId);
            var isPaymentMethodValid = model.PaymentType switch
            {
                PaymentMethod.BankAccount => await this.bankAccountService.UserOwnsAccount(model.PaymentMethod, userId),
                PaymentMethod.CreditOrDebitCard => await this.creditCardService.UserOwnsCard(model.PaymentMethod, userId),
                _ => false,
            };

            if (!userOwnsAccount || !isPaymentMethodValid)
            {
                return this.BadRequest();
            }

            var serviceModel = new DepositServiceModel()
            {
                Amount = model.Amount,
                Fee = GlobalConstants.DefaultDepositFee,
                RecipientAddress = model.Account,
            };

            if (!await this.transferService.DepositToAccount(userId, serviceModel))
            {
                this.ModelState.AddModelError(string.Empty, "Something went wrong try again.");
                return this.View(nameof(this.Deposit));
            }

            if (model.PaymentType is PaymentMethod.BankAccount)
            {
                await this.bankAccountService.ConfirmAccount(model.PaymentMethod);
            }

            return this.Redirect($"/{this.ControllerName}/{nameof(this.DepositSuccess)}");
        }

        public IActionResult DepositSuccess() => this.View();

        public async Task<IActionResult> Withdraw()
        {
            var model = await this.GetWithdrawViewModel();

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Withdraw(WithdrawInputModel model)
        {
            var userId = this.userManager.GetUserId(this.User);

            if (!await this.bankAccountService.Exists(model.BankAccount)
                || !await this.bankAccountService.UserOwnsAccount(model.BankAccount, userId)
                || !await this.bankAccountService.AccountIsConfirmed(model.BankAccount))
            {
                this.ModelState.AddModelError(string.Empty, WebConstants.WithdrawInputModel.BankAccountErrorMessage);
            }

            if (!await this.accountService.Exists(model.CoinAccount)
                || !await this.accountService.UserOwnsAccount(model.CoinAccount, userId))
            {
                this.ModelState.AddModelError(string.Empty, WebConstants.WithdrawInputModel.CoinAccountErrorMessage);
            }

            if (!await this.accountService.HasSufficientFunds(model.CoinAccount, model.Amount))
            {
                this.ModelState.AddModelError(string.Empty, WebConstants.WithdrawInputModel.InsufficientFundsErrorMessage);
            }

            if (!this.ModelState.IsValid)
            {
                var viewModel = await this.GetWithdrawViewModel();

                return this.View(viewModel);
            }

            var serviceModel = this.mapper.Map<WithdrawServiceModel>(model);

            if (!await this.transferService.WithdrawFromAccount(userId, serviceModel))
            {
                this.ModelState.AddModelError(string.Empty, "Something went wrong try again.");
                var viewModel = await this.GetWithdrawViewModel();

                return this.View(viewModel);
            }

            var controller = ControllerHelpers.GetControllerName<TransfersController>();
            return this.Redirect($"/{controller}/{nameof(this.WithdrawSuccess)}");
        }

        public IActionResult WithdrawSuccess() => this.View();

        private async Task<WithdrawViewModel> GetWithdrawViewModel()
        {
            var userId = this.userManager.GetUserId(this.User);
            var model = await this.userService.GetUser<WithdrawViewModel>(userId);

            return model;
        }
    }
}
