namespace PaymentSystem.WalletApp.Web.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using PaymentSystem.WalletApp.Common;
    using PaymentSystem.WalletApp.Web.Controllers;

    [Authorize(Roles = WalletConstants.AdministratorRoleName)]
    [Area("Administration")]
    public class AdministrationController : BaseController
    {
    }
}
