namespace PaymentSystem.WalletApp.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels.Transfers;

    [Authorize]
    public class TransfersController : Controller
    {
        public async Task<IActionResult> Deposit()
        {
            return await Task.FromResult(this.View());
        }

        [HttpPost]
        public async Task<IActionResult> Deposit(DepositModel model)
        {
            return await Task.FromResult(this.View());
        }

        public async Task<IActionResult> Withdraw()
        {
            return await Task.FromResult(this.View());
        }
    }
}
