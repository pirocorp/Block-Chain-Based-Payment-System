namespace PaymentSystem.WalletApp.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services.Data;
    using PaymentSystem.WalletApp.Web.ViewModels.Accounts.Index;
    using PaymentSystem.WalletApp.Web.ViewModels.Accounts.Transactions;

    using static Infrastructure.WebConstants;

    [Authorize]
    public class AccountsController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IAccountService accountService;
        private readonly ITransactionService transactionService;

        public AccountsController(
            UserManager<ApplicationUser> userManager,
            IAccountService accountService,
            ITransactionService transactionService)
        {
            this.userManager = userManager;
            this.accountService = accountService;
            this.transactionService = transactionService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = this.userManager.GetUserId(this.User);

            var model = new AccountsIndexVewModel()
            {
                Accounts = await this.accountService.GetUserAccounts<AccountIndexListingModel>(userId),
            };

            return this.View(model);
        }

        public async Task<IActionResult> Transactions(string address, int page = 1)
        {
            page = Math.Max(page, 1);

            var userId = this.userManager.GetUserId(this.User);

            if (!await this.accountService.UserOwnsAccount(address, userId))
            {
                return this.RedirectToAction(nameof(this.Index));
            }

            var (totalTransactions, transactions) = await this.transactionService
                .GetTransactionsForAddress<AccountsTransactionsListingModel>(address, page, DefaultTransactionsResultPageSize);

            var user = await this.userManager.GetUserAsync(this.User);

            transactions = transactions.ToList();

            foreach (var transaction in transactions)
            {
                transaction.CurrentAccountAddress = address;
            }

            var model = new AccountsTransactionsViewModel()
            {
                Transactions = transactions,
                CurrentAccountAddress = address,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalTransactions / (double)DefaultTransactionsResultPageSize),
            };

            return this.View(model);
        }
    }
}
