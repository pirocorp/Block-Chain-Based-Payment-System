namespace PaymentSystem.WalletApp.Web.Areas.Administration.Controllers
{
    using PaymentSystem.WalletApp.Common;
    using PaymentSystem.WalletApp.Web.Controllers;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Roles = WalletConstants.AdministratorRoleName)]
    [Area("Administration")]
    public class AdministrationController : BaseController
    {
    }
}
