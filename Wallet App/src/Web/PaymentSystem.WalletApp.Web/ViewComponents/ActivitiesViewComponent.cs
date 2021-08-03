namespace PaymentSystem.WalletApp.Web.ViewComponents
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using PaymentSystem.WalletApp.Web.ViewModels;

    public class ActivitiesViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(IEnumerable<ActivityListingModel> model)
            => await Task.FromResult(this.View(model));
    }
}
