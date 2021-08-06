namespace PaymentSystem.WalletApp.Web.ViewModels.Transactions.SendCoins
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.AspNetCore.Mvc.Rendering;
    using PaymentSystem.WalletApp.Web.ViewModels.Profile.Accounts;

    public class SendCoinsViewModel
    {
        public IEnumerable<CoinAccountModel> Accounts { get; set; }

        public IEnumerable<SelectListItem> Coins => this.Accounts
            .Select(c => new SelectListItem($"{c.Address[^12..]} - {c.Balance:F2}", c.Address));
    }
}
