namespace PaymentSystem.WalletApp.Web.Infrastructure.Filters.ActionFilters
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    using PaymentSystem.WalletApp.Web.Infrastructure.Helpers;

    /// <summary>
    /// The ExportModelStateAttribute runs after an Action has executed,
    /// checks whether the ModelState was invalid and if the returned result
    /// was a redirect result. If it was, then it serializes the ModelState
    /// and stores it in TempData.
    /// </summary>
    public class ExportModelStateAttribute : ModelStateTransfer
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // Only export when ModelState is not valid
            if (!filterContext.ModelState.IsValid)
            {
                // Export if we are redirecting
                if (filterContext.Result is RedirectResult or RedirectToRouteResult or RedirectToActionResult)
                {
                    if (filterContext.Controller is Controller controller && filterContext.ModelState != null)
                    {
                        var modelState = ModelStateHelpers.SerializeModelState(filterContext.ModelState);
                        controller.TempData[Key] = modelState;
                    }
                }
            }

            base.OnActionExecuted(filterContext);
        }
    }
}
