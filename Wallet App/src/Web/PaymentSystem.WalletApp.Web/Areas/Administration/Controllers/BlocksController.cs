namespace PaymentSystem.WalletApp.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using PaymentSystem.WalletApp.Services.Data;
    using PaymentSystem.WalletApp.Web.ViewModels.Administration.Blocks;
    using PaymentSystem.WalletApp.Web.ViewModels.Administration.Transactions;

    using static Infrastructure.WebConstants;

    public class BlocksController : AdministrationController
    {
        private readonly IBlockService blockService;
        private readonly ITransactionService transactionService;

        public BlocksController(
            IBlockService blockService,
            ITransactionService transactionService)
        {
            this.blockService = blockService;
            this.transactionService = transactionService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var currentPageSize = DefaultBlocksResultPageSize;

            var (totalPages, blocks) = await this.Pagination(
                this.blockService.GetBlocks<BlockListingAdminModel>,
                page,
                currentPageSize);

            var model = new BlockIndexAdminViewModel()
            {
                Blocks = blocks,
                CurrentPage = page,
                TotalPages = totalPages,
            };

            return this.View(model);
        }

        public async Task<IActionResult> Details(string hash, int page = 1)
        {
            if (hash is null)
            {
                return this.RedirectToAction<AdministrationController, DashboardController>(nameof(DashboardController.Index));
            }

            var currentPageSize = DefaultTransactionsResultPageSize;

            var block = await this.blockService.GetBlock<BlockDetailsAdminModel>(hash);

            var (totalPages, transactions) = await this
                .Pagination(
                    async (p, c) => await this.transactionService.GetBlockTransactions<TransactionListingAdminModel>(hash, p, c),
                    page,
                    currentPageSize);

            var model = new BlockDetailsAdminViewModel()
            {
                Block = block,
                CurrentPage = page,
                TotalPages = totalPages,
                Transactions = transactions,
            };

            return this.View(model);
        }
    }
}
