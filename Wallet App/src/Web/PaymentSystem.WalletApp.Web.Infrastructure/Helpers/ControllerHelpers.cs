namespace PaymentSystem.WalletApp.Web.Infrastructure.Helpers
{
    using System;

    using Microsoft.AspNetCore.Mvc;

    public static class ControllerHelpers
    {
        public static string GetControllerName<T>()
            where T : Controller
            => typeof(T).Name.Replace(nameof(Controller), string.Empty);
    }
}
