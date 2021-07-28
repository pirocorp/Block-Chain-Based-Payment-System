namespace PaymentSystem.WalletApp.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using PaymentSystem.WalletApp.Web.Infrastructure.Helpers;

    public class BaseController : Controller
    {
        public string ControllerName => ControllerHelpers.GetControllerName(this.GetType());
    }
}
