namespace PaymentSystem.WalletApp.Web.Controllers
{
    using System.Threading.Tasks;

    using AutoMapper;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using Newtonsoft.Json;

    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services.Data;
    using PaymentSystem.WalletApp.Web.Infrastructure.Filters.ActionFilters;
    using PaymentSystem.WalletApp.Web.ViewModels.Profile.Accounts;
    using PaymentSystem.WalletApp.Web.ViewModels.Transactions;
    using PaymentSystem.WalletApp.Web.ViewModels.Transactions.SendCoins;
    using PaymentSystem.WalletApp.Web.ViewModels.Transactions.SendCoinsConfirm;

    using static Infrastructure.WebConstants.SendCoinErrorMessages;

    [Authorize]
    public class TransactionsController : BaseController
    {
        private const string SendCoinTempData = "Send Coin Temp Data";

        private readonly IAccountService accountService;
        private readonly IMapper mapper;
        private readonly ITransactionService transactionService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserService userService;

        public TransactionsController(
            IAccountService accountService,
            IMapper mapper,
            ITransactionService transactionService,
            UserManager<ApplicationUser> userManager,
            IUserService userService)
        {
            this.accountService = accountService;
            this.mapper = mapper;
            this.transactionService = transactionService;
            this.userManager = userManager;
            this.userService = userService;
        }

        public async Task<IActionResult> GetTransactionDetails(string id)
        {
            var activity = await this.transactionService
                .GetTransaction<TransactionDetails>(id);

            return this.Ok(activity);
        }

        [ImportModelState]
        public async Task<IActionResult> SendCoins()
        {
            var userId = this.userManager.GetUserId(this.User);

            var model = new SendCoinsViewModel()
            {
                Accounts = await this.accountService.GetUserAccounts<CoinAccountModel>(userId),
            };

            return this.View(model);
        }

        [HttpPost]
        [ExportModelState]
        public async Task<IActionResult> SendCoins(SendCoinInputModel model)
        {
            var userId = this.userManager.GetUserId(this.User);

            if (!await this.accountService.UserOwnsAccount(model.CoinAccount, userId))
            {
                this.ModelState.AddModelError(string.Empty, CoinAccountErrorMessage);
            }
            else if (!await this.accountService.HasSufficientFunds(model.CoinAccount, model.Amount))
            {
                this.ModelState.AddModelError(string.Empty, InsufficientFundsErrorMessage);
            }

            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction(nameof(this.SendCoins));
            }

            this.TempData[SendCoinTempData] = JsonConvert.SerializeObject(model);

            return this.RedirectToAction<TransactionsController>(nameof(this.SendCoinsConfirm));
        }

        public IActionResult SendCoinsConfirm()
        {
            if (!this.TempData.ContainsKey(SendCoinTempData))
            {
                return this.Redirect($"/{this.ControllerName}/{nameof(this.SendCoins)}");
            }

            var serializedObject = (string)this.TempData[SendCoinTempData];
            var sendCoinInputModel = JsonConvert.DeserializeObject<SendCoinInputModel>(serializedObject);
            var viewModel = this.mapper.Map<SendCoinsConfirmViewModel>(sendCoinInputModel);

            return this.View(viewModel);
        }

        [HttpPost]
        [ExportModelState]
        public async Task<IActionResult> SendCoinsConfirm(SendCoinConfirmInputModel model)
        {
            var userId = this.userManager.GetUserId(this.User);

            if (!await this.accountService.UserOwnsAccount(model.CoinAccount, userId))
            {
                this.ModelState.AddModelError(string.Empty, CoinAccountErrorMessage);
            }
            else if (!await this.accountService.HasSufficientFunds(model.CoinAccount, model.Amount))
            {
                this.ModelState.AddModelError(string.Empty, InsufficientFundsErrorMessage);
            }

            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction(nameof(this.SendCoins));
            }

            await this.userService.SendCoins(model.CoinAccount, model.Recipient, model.Amount, model.Secret, userId);

            return this.RedirectToAction(nameof(this.SendCoinsSuccess));
        }

        public IActionResult SendCoinsSuccess() => this.View();
    }
}
