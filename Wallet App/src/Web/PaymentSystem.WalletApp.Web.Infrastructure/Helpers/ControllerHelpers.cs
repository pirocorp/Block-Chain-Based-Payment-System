namespace PaymentSystem.WalletApp.Web.Infrastructure.Helpers
{
    using System;

    using Microsoft.AspNetCore.Mvc;

    public static class ControllerHelpers
    {
        public static string GetControllerName<T>()
            where T : Controller
            => GetName(typeof(T));

        public static string GetControllerName(Type controllerType)
        {
            if (!controllerType.IsSubclassOf(typeof(Controller)))
            {
                throw new NotSupportedException($"{controllerType.Name} must inherit Controller.");
            }

            return GetName(controllerType);
        }

        private static string GetName(Type type)
            => type.Name.Replace(nameof(Controller), string.Empty);
    }
}
