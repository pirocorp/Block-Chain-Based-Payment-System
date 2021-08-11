namespace PaymentSystem.WalletApp.Web.Areas.Administration.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using PaymentSystem.WalletApp.Services.Data;
    using PaymentSystem.WalletApp.Web.ViewModels.Administration.Accounts;

    using static Infrastructure.WebConstants;

    public class AccountsController : AdministrationController
    {
        private readonly IAccountService accountService;

        public AccountsController(IAccountService accountService)
        {
            this.accountService = accountService;
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

        public async Task<IActionResult> Details(string address)
        {
            return this.Ok(address);
        }
    }
}
