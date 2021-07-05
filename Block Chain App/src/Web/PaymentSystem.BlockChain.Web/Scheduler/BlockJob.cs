namespace PaymentSystem.BlockChain.Web.Scheduler
{
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;
    using Coravel.Invocable;
    using Microsoft.AspNetCore.SignalR;

    using PaymentSystem.BlockChain.Services.Data;
    using PaymentSystem.BlockChain.Services.Hubs;
    using PaymentSystem.Common.Hubs;
    using PaymentSystem.Common.Hubs.Models;

    public class BlockJob : IInvocable
    {
        private readonly IMapper mapper;
        private readonly ICancelTransactionPool cancelTransactionPool;
        private readonly IBlockChainService blockChainService;
        private readonly IHubContext<BroadcastHub, IBlockNotificationClient> broadcastHubContext;

        public BlockJob(
            IMapper mapper,
            ICancelTransactionPool cancelTransactionPool,
            IBlockChainService blockChainService,
            IHubContext<BroadcastHub, IBlockNotificationClient> broadcastHubContext)
        {
            this.mapper = mapper;
            this.cancelTransactionPool = cancelTransactionPool;
            this.blockChainService = blockChainService;
            this.broadcastHubContext = broadcastHubContext;
        }

        public async Task Invoke()
        {
            var block = await this.blockChainService.MineBlock();

            if (block is null)
            {
                return;
            }

            var notificationBlock = this.mapper.Map<NotificationBlock>(block);

            notificationBlock.CanceledTransactions = this.cancelTransactionPool.GetAll()
                .Select(t => this.mapper.Map<TransactionNotification>(t));

            await this.broadcastHubContext.Clients.All.ReceiveBlock(notificationBlock);
        }
    }
}
