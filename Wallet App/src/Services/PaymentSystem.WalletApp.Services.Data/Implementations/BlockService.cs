namespace PaymentSystem.WalletApp.Services.Data.Implementations
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    using PaymentSystem.Common.Data.Models;
    using PaymentSystem.Common.Hubs.Models;
    using PaymentSystem.WalletApp.Data;
    using PaymentSystem.WalletApp.Data.Models;
    using PaymentSystem.WalletApp.Services.Data;
    using PaymentSystem.WalletApp.Services.Data.Models.Activities;
    using PaymentSystem.WalletApp.Web.Infrastructure;
    using PaymentSystem.WalletApp.Web.Infrastructure.Options;

    public class BlockService : IBlockService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IBlockChainGrpcService blockChainGrpcService;
        private readonly IActivityService activityService;
        private readonly IAccountService accountService;
        private readonly IOptions<WalletProviderOptions> walletProviderOptions;

        public BlockService(
            ApplicationDbContext dbContext,
            IBlockChainGrpcService blockChainGrpcService,
            IActivityService activityService,
            IAccountService accountService,
            IOptions<WalletProviderOptions> walletProviderOptions)
        {
            this.dbContext = dbContext;
            this.blockChainGrpcService = blockChainGrpcService;
            this.activityService = activityService;
            this.accountService = accountService;
            this.walletProviderOptions = walletProviderOptions;
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

                    if (transaction.Recipient != this.walletProviderOptions.Value.Address &&
                        transaction.Sender != this.walletProviderOptions.Value.Address)
                    {
                        var userId = await this.accountService.GetUserId(transaction.Recipient);
                        var model = new ActivityServiceModel()
                        {
                            Amount = transaction.Amount + transaction.Fee,
                            BlockedAmount = 0,
                            CounterpartyAddress = transaction.Sender,
                            Description = string.Format(WebConstants.ReceiveDescription, transaction.Amount, transaction.Sender),
                            Status = ActivityStatus.Completed,
                            TransactionHash = transaction.Hash,
                            UserId = userId,
                        };

                        await this.activityService.AddActivity(model);
                    }
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
                    await this.activityService.ReturnBlockedAmount(transaction.Hash);
                    await this.activityService.SetActivityStatus(transaction.Hash, ActivityStatus.Canceled);
                }
            }
        }
    }
}
