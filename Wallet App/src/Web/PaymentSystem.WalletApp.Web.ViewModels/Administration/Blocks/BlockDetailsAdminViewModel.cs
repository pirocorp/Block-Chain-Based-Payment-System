namespace PaymentSystem.WalletApp.Web.ViewModels.Administration.Blocks
{
    using System.Collections.Generic;

    using PaymentSystem.WalletApp.Web.ViewModels.Administration.Transactions;

    public class BlockDetailsAdminViewModel
    {
        public BlockDetailsAdminModel Block { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public IEnumerable<TransactionListingAdminModel> Transactions { get; set; }
    }
}
