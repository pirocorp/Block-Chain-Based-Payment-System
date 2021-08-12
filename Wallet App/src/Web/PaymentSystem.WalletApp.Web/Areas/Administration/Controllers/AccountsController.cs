namespace PaymentSystem.WalletApp.Web.Areas.Administration.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using PaymentSystem.WalletApp.Services.Data;
    using PaymentSystem.WalletApp.Web.ViewModels.Administration.Accounts;
    using PaymentSystem.WalletApp.Web.ViewModels.Administration.Transactions;

    using static Infrastructure.WebConstants;

    public class AccountsController : AdministrationController
    {
        private readonly IAccountService accountService;
        private readonly ITransactionService transactionService;

        public AccountsController(
            IAccountService accountService,
            ITransactionService transactionService)
        {
            this.accountService = accountService;
            this.transactionService = transactionService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var currentPageSize = DefaultAccountsResultPageSize;

            var (totalPages, accounts) = await this
                .Pagination(
                    this.accountService.GetAccounts<AccountListingAdminModel>,
                    page,
                    currentPageSize);

            var model = new AccountIndexAdminViewModel()
            {
                Accounts = accounts,
                CurrentPage = page,
                TotalPages = totalPages,
            };

            return this.View(model);
        }

        public async Task<IActionResult> Details(string address, int page = 1)
        {
            if (address is null)
            {
                return this.RedirectToAction<AdministrationController, DashboardController>(
                    nameof(DashboardController.Index));
            }

            var currentPageSize = DefaultTransactionsResultPageSize;

            var (totalPages, transactions) = await this.Pagination(
                async (p, c) => await this.transactionService
                    .GetAccountTransactions<TransactionListingAdminModel>(address, p, c),
                page,
                currentPageSize
                );

            var (inflow, outflow) = await this.transactionService.GetAccountMoneyFlow(address);

            var account = await this.accountService.GetAccount<AccountDetailsModel>(address);

            if (account is null)
            {
                account = new AccountDetailsModel
                {
                    AccountAddress = address,
                };
            }

            account.TotalInflow = inflow;
            account.TotalOutflow = outflow;

            var model = new AccountDetailsAdminViewModel()
            {
                Account = account,
                CurrentPage = page,
                TotalPages = totalPages,
                Transactions = transactions,
            };

            return this.View(model);
        }
    }
}
