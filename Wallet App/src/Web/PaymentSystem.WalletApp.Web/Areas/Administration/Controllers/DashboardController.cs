namespace PaymentSystem.WalletApp.Web.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using PaymentSystem.WalletApp.Web.ViewModels.Administration.Dashboard;

    public class DashboardController : AdministrationController
    {
        public DashboardController()
        {
        }

        public IActionResult Index()
        {
            return this.View();
        }
    }
}
