namespace PaymentSystem.WalletApp.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    public class AccountsController : AdministrationController
    {
        public async Task<IActionResult> Index()
        {
            return this.Ok();
        }

        public async Task<IActionResult> Details(string address)
        {
            return this.Ok(address);
        }
    }
}
