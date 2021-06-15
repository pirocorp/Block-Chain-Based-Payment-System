namespace PaymentSystem.BlockChain.Services
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.SignalR;

    using PaymentSystem.BlockChain.Services.Hubs;
    using PaymentSystem.Common.Data.Models;
    using PaymentSystem.Common.Hubs;
    using PaymentSystem.Common.Hubs.Models;

    public class BroadcastService
    {
        private readonly IHubContext<BroadcastHub, IBroadcastClient> broadcastHub;

        public BroadcastService(IHubContext<BroadcastHub, IBroadcastClient> broadcastHub)
        {
            this.broadcastHub = broadcastHub;
        }

        // This method will be called on background task when new block is created;
        public async Task SendBlock(Block block)
        {
            var notificationHeader = new NotificationHeader()
            {
                Version = block.BlockHeader.Version,
                PreviousHash = block.BlockHeader.PreviousHash,
                MerkleRoot = block.BlockHeader.MerkleRoot,
                TimeStamp = block.BlockHeader.TimeStamp,
                Difficulty = block.BlockHeader.Difficulty,
                Validator = block.BlockHeader.Validator,
            };

            var notificationBlock = new NotificationBlock()
            {
                Hash = block.Hash,
                Height = block.Height,
                BlockHeader = notificationHeader,
                Transactions = block.Transactions.Select(ConvertTransaction).ToArray(),
                Validator = block.Validator,
            };

            await this.broadcastHub.Clients.All.ReceiveBlock(notificationBlock);
        }

        private static TransactionNotification ConvertTransaction(Transaction transaction)
        {
            var transactionNotification = new TransactionNotification()
            {
                Hash = transaction.Hash,
                TimeStamp = transaction.TimeStamp,
                Sender = transaction.Sender,
                Recipient = transaction.Recipient,
                Amount = transaction.Amount,
                Fee = transaction.Fee,
                BlockHash = transaction.BlockHash,
            };

            return transactionNotification;
        }
    }
}
