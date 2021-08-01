namespace PaymentSystem.WalletApp.Web.Controllers
{
    using System.Threading.Tasks;
    using Infrastructure;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using Newtonsoft.Json;

    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Web.ViewModels.Transfers.Deposit;
    using PaymentSystem.WalletApp.Web.ViewModels.Transfers.DepositConfirm;
    using PaymentSystem.WalletApp.Services.Data;
    using Services.Data.Models;
    using Services.Data.Models.Activities;
    using Services.Data.Models.Transactions;

    [Authorize]
    public class TransfersController : BaseController
    {
        private const string DepositTempData = "depositTempData";

        private readonly UserManager<ApplicationUser> userManager;
        private readonly IAccountService accountService;
        private readonly IBankAccountService bankAccountService;
        private readonly ICreditCardService creditCardService;
        private readonly ITransferService transferService;
        private readonly IUserService userService;
        
        public TransfersController(
            UserManager<ApplicationUser> userManager,
            IAccountService accountService,
            IBankAccountService bankAccountService,
            ICreditCardService creditCardService,
            ITransferService transferService,
            IUserService userService)
        {
            this.userManager = userManager;
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
            var userOwnsAccount = await this.accountService.UserOwnsAccount(userId, model.Account);
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

            var serviceModel = new CreateTransactionServiceModel()
            {
                Amount = model.Amount,
                RecipientAddress = model.Account,
            };

            if (!await this.transferService.DepositToAccount(userId, serviceModel))
            {
                this.ModelState.AddModelError(string.Empty, "Something went wrong try again.");
                return this.View(nameof(this.Deposit));
            }

            return this.Redirect($"/{this.ControllerName}/{nameof(this.DepositSuccess)}");
        }

        public IActionResult DepositSuccess() => this.View();

        public async Task<IActionResult> Withdraw()
        {
            return await Task.FromResult(this.View());
        }
    }
}
