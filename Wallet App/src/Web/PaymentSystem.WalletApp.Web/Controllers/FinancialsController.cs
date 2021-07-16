namespace PaymentSystem.WalletApp.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class FinancialsController : BaseController
    {
        public async Task<IActionResult> Profile()
        {
            return this.View();
        }
    }
}
