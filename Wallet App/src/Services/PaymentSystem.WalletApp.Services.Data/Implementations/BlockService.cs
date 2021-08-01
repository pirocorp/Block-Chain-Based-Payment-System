namespace PaymentSystem.WalletApp.Services.Data.Implementations
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using PaymentSystem.Common.Data.Models;
    using PaymentSystem.Common.Hubs.Models;
    using PaymentSystem.WalletApp.Data;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services.Data;

    public class BlockService : IBlockService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IBlockChainGrpcService blockChainGrpcService;
        private readonly IActivityService activityService;
        private readonly IAccountService accountService;

        public BlockService(
            ApplicationDbContext dbContext,
            IBlockChainGrpcService blockChainGrpcService,
            IActivityService activityService,
            IAccountService accountService)
        {
            this.dbContext = dbContext;
            this.blockChainGrpcService = blockChainGrpcService;
            this.activityService = activityService;
            this.accountService = accountService;
        }

        public async Task<Block> GetLastBlock() => await this.dbContext.Blocks
            .OrderByDescending(b => b.Height)
            .FirstOrDefaultAsync();

        public async Task ReceiveBlock(Block notificationBlock, IEnumerable<CanceledTransaction> canceledTransactions)
        {
            var lastBlock = await this.GetLastBlock();

            // We have missing blocks
            if (lastBlock is null)
            {
                await this.GetMissingBlocks(0, notificationBlock.Height - 1);
            }
            else if (lastBlock.Height < notificationBlock.Height - 1)
            {
                await this.GetMissingBlocks(lastBlock.Height + 1, notificationBlock.Height - 1);
            }
            else if (notificationBlock.Height == lastBlock.Height)
            {
                return;
            }

            // Process current last block.
            await this.ProcessBlock(notificationBlock);
            await this.ProcessCanceledTransactions(canceledTransactions);
        }

        private async Task GetMissingBlocks(long start, long end)
        {
            for (var i = start; i <= end; i++)
            {
                var block = await this.blockChainGrpcService.GetBlock(i);
                await this.ProcessBlock(block);
            }
        }

        private async Task ProcessBlock(Block block)
        {
            await this.ProcessTransactions(block.Transactions);

            await this.dbContext.AddAsync(block);
            await this.dbContext.SaveChangesAsync();
        }

        private async Task ProcessTransactions(IEnumerable<Transaction> transactions)
        {
            foreach (var transaction in transactions)
            {
                if (await this.activityService.Exists(transaction.Hash))
                {
                    await this.activityService.SetActivityStatus(transaction.Hash, ActivityStatus.Completed);
                }

                if (await this.accountService.Exists(transaction.Recipient))
                {
                    await this.accountService.Deposit(transaction.Recipient, transaction.Amount);
                }

                if (await this.accountService.Exists(transaction.Sender))
                {
                    await this.accountService
                        .Withdraw(transaction.Sender, transaction.Amount + transaction.Fee);
                }
            }
        }

        private async Task ProcessCanceledTransactions(IEnumerable<CanceledTransaction> canceledTransactions)
        {
            foreach (var transaction in canceledTransactions)
            {
                if (await this.activityService.Exists(transaction.Hash))
                {
                    await this.activityService.SetActivityStatus(transaction.Hash, ActivityStatus.Canceled);
                }
            }
        }
    }
}
