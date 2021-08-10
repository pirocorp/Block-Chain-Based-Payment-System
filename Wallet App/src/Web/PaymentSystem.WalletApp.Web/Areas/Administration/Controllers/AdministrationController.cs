namespace PaymentSystem.WalletApp.Web.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using PaymentSystem.WalletApp.Common;
    using PaymentSystem.WalletApp.Web.Controllers;

    [Area("Administration")]
    [Authorize(Roles = WalletConstants.AdministratorRoleName)]
    public class AdministrationController : BaseController
    {
    }
}
