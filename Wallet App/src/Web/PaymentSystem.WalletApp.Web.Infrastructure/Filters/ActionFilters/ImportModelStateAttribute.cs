namespace PaymentSystem.WalletApp.Web.Infrastructure.Filters.ActionFilters
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    using PaymentSystem.WalletApp.Web.Infrastructure.Helpers;

    /// <summary>
    /// The ImportModelStateAttribute also runs after an Action has executed,
    /// checks we have a serialized model state and that we are going to execute
    /// a ViewResult. If so, then it deserializes to state to a ModelStateDictionary
    /// and merges it into the existing ModelState.
    /// </summary>
    public class ImportModelStateAttribute : ModelStateTransfer
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var controller = filterContext.Controller as Controller;

            if (controller?.TempData[Key] is string serializedModelState)
            {
                // Only Import if we are viewing
                if (filterContext.Result is ViewResult)
                {
                    var modelState = ModelStateHelpers.DeserializeModelState(serializedModelState);
                    filterContext.ModelState.Merge(modelState);
                }
                else
                {
                    // Otherwise remove it.
                    controller.TempData.Remove(Key);
                }
            }

            base.OnActionExecuted(filterContext);
        }
    }
}
