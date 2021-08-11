namespace PaymentSystem.WalletApp.Web.Areas.Administration.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using PaymentSystem.WalletApp.Common;
    using PaymentSystem.WalletApp.Web.Controllers;

    [Area("Administration")]
    [Authorize(Roles = WalletConstants.AdministratorRoleName)]
    public class AdministrationController : BaseController
    {
        protected async Task<(int TotalPages, IEnumerable<TItem>)> Pagination<TItem>(Func<int, int, Task<(int Total, IEnumerable<TItem>)>> func, int page, int pageSize)
        {
            page = Math.Max(1, page);

            var (total, items) = await func(page, pageSize);

            var totalPages = (int)Math.Ceiling(total / (double)pageSize);

            return (totalPages, items);
        }
    }
}
