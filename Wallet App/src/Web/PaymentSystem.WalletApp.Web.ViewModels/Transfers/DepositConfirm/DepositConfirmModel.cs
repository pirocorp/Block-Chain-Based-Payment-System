namespace PaymentSystem.WalletApp.Web.ViewModels.Transfers.DepositConfirm
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Mvc.Rendering;

    using PaymentSystem.WalletApp.Services.Data.Models;

    public class DepositConfirmModel
    {
        public IEnumerable<SelectListItem> PaymentMethods { get; set; }

        public IEnumerable<SelectListItem> Accounts { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public double Amount { get; set; }

        public double Total { get; set; }
    }
}
