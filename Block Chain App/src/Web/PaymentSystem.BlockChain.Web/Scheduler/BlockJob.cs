namespace PaymentSystem.BlockChain.Web.Scheduler
{
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;
    using Common.Utilities;
    using Coravel.Invocable;
    using Microsoft.AspNetCore.SignalR;

    using PaymentSystem.BlockChain.Services.Data;
    using PaymentSystem.BlockChain.Services.Hubs;
    using PaymentSystem.Common.Hubs;
    using PaymentSystem.Common.Hubs.Models;

    public class BlockJob : IInvocable
    {
        private readonly SystemKeys systemKeys;
        private readonly IHubContext<BroadcastHub, IBlockNotificationClient> broadcastHubContext;
        private readonly IBlockChainService blockChainService;
        private readonly ICancelTransactionPool cancelTransactionPool;
        private readonly IMapper mapper;

        public BlockJob(
            SystemKeys systemKeys,
            IHubContext<BroadcastHub, IBlockNotificationClient> broadcastHubContext,
            IBlockChainService blockChainService,
            ICancelTransactionPool cancelTransactionPool, 
            IMapper mapper)
        {
            this.systemKeys = systemKeys;
            this.broadcastHubContext = broadcastHubContext;
            this.blockChainService = blockChainService;
            this.cancelTransactionPool = cancelTransactionPool;
            this.mapper = mapper;
        }

        public async Task Invoke()
        {
            var block = await this.blockChainService.MineBlock();

            if (block is null)
            {
                return;
            }

            var notificationBlock = this.mapper.Map<NotificationBlock>(block);

            notificationBlock.BlockChainPublicKey = this.systemKeys.PublicKey.PublicKeyToString();
            notificationBlock.BlockChainSignature = BlockChainHashing
                .CreateSignature(notificationBlock.Hash, this.systemKeys.PrivateKey);

            notificationBlock.CanceledTransactions = this.cancelTransactionPool.GetAll()
                .Select(t => this.mapper.Map<TransactionNotification>(t));

            await this.broadcastHubContext.Clients.All.ReceiveBlock(notificationBlock);
        }
    }
}
