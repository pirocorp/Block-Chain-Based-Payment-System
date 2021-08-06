namespace PaymentSystem.WalletApp.Web.Controllers
{
    using System;

    using Microsoft.AspNetCore.Mvc;

    using PaymentSystem.WalletApp.Web.Infrastructure.Helpers;

    public class BaseController : Controller
    {
        protected string ControllerName => GetControllerName(this.GetType());

        protected IActionResult RedirectToAction<TArea, TController>(string action)
        {
            var area = ControllerHelpers.GetAreaName(typeof(TArea));

            return this.RedirectToAction(action, GetControllerName(typeof(TController)), new { area = area });
        }

        protected IActionResult RedirectToAction<TController>(string action)
            => this.RedirectToAction(action, GetControllerName(typeof(TController)));

        private static string GetControllerName(Type t) => ControllerHelpers.GetControllerName(t);
    }
}
