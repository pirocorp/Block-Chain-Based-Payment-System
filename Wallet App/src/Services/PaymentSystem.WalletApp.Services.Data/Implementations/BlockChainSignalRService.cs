namespace PaymentSystem.WalletApp.Services.Data.Implementations
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.SignalR.Client;
    using Microsoft.Extensions.DependencyInjection;

    using PaymentSystem.Common;
    using PaymentSystem.Common.Data.Models;
    using PaymentSystem.Common.Hubs;
    using PaymentSystem.Common.Hubs.Models;
    using PaymentSystem.Common.Utilities;
    using PaymentSystem.WalletApp.Services.Data;

    public class BlockChainSignalRService : IBlockChainSignalRService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly HubConnection connection;

        public BlockChainSignalRService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.connection = new HubConnectionBuilder()
                .WithUrl($"{GlobalConstants.ChanelAddress}{GlobalConstants.PushNotificationUrl}")
                .WithAutomaticReconnect()
                .Build();
        }

        public async Task Run()
        {
            await this.connection.StartAsync();

            this.connection.On<NotificationBlock>(nameof(IBlockNotificationClient.ReceiveBlock), notificationBlock =>
            {
                this.ProcessBlock(notificationBlock)
                    .GetAwaiter()
                    .GetResult();
            });
        }

        private static Block NotificationBlockToBlock(NotificationBlock notificationBlock)
        {
            var block = new Block()
            {
                BlockHeader = new BlockHeader()
                {
                    Difficulty = notificationBlock.BlockHeader.Difficulty,
                    TimeStamp = notificationBlock.BlockHeader.TimeStamp,
                    MerkleRoot = notificationBlock.BlockHeader.MerkleRoot,
                    PreviousHash = notificationBlock.BlockHeader.PreviousHash,
                    Validator = notificationBlock.BlockHeader.Validator,
                    Version = notificationBlock.BlockHeader.Version,
                },
                Hash = notificationBlock.Hash,
                Height = notificationBlock.Height,
                Transactions = notificationBlock
                    .Transactions
                    .Select(t => new Transaction()
                    {
                        Amount = t.Amount,
                        BlockHash = t.BlockHash,
                        Fee = t.Fee,
                        Hash = t.Hash,
                        Recipient = t.Recipient,
                        Sender = t.Sender,
                        TimeStamp = t.TimeStamp,
                    })
                    .ToList(),
            };
            return block;
        }

        /// <summary>
        /// Verifies and convert notification block into necessary type.
        /// </summary>
        /// <param name="notificationBlock">Notification block.</param>
        private async Task ProcessBlock(NotificationBlock notificationBlock)
        {
            using var scope = this.serviceProvider.CreateScope();

            // Dependencies are resolved manually because otherwise will have captured dependencies;
            var blockService = scope.ServiceProvider.GetRequiredService<IBlockService>();
            var settingService = scope.ServiceProvider.GetRequiredService<ISettingService>();

            var blockchainPublicKey = await settingService.GetBlockPublicKey();

            if (!this.ValidateSignature(notificationBlock, blockchainPublicKey)
                || !this.ValidateTransactions(notificationBlock)
                || !this.ValidateBlock(notificationBlock))
            {
                return;
            }

            var block = NotificationBlockToBlock(notificationBlock);
            var canceledTransactions = notificationBlock.CanceledTransactions;

            await blockService.ReceiveBlock(block, canceledTransactions);
        }

        /// <summary>
        /// Validate that block is send by the blockchain.
        /// </summary>
        /// <param name="block">Notification block.</param>
        /// <param name="blockchainAddress">Blockchain Public Key.</param>
        /// <returns></returns>
        private bool ValidateSignature(NotificationBlock block, string blockchainAddress)
            => BlockChainHashing.VerifySignature(blockchainAddress, block.Hash, block.BlockChainSignature);

        /// <summary>
        /// Validates transactions Merkle root.
        /// No one is put unauthorized transaction.
        /// </summary>
        /// <param name="block">Notification block.</param>
        /// <returns></returns>
        private bool ValidateTransactions(NotificationBlock block)
        {
            var hashes = block.Transactions
                .Select(t => t.Hash);

            var calculatedMerkleRoot = BlockChainHashing.GenerateMerkleRoot(hashes);
            return calculatedMerkleRoot == block.BlockHeader.MerkleRoot;
        }

        /// <summary>
        /// Validate that given block is a signed block.
        /// </summary>
        /// <param name="block">Notification block.</param>
        /// <returns></returns>
        private bool ValidateBlock(NotificationBlock block)
        {
            var calculatedHash = BlockChainHashing.GenerateBlockHash(
                block.BlockHeader.Version,
                block.BlockHeader.PreviousHash,
                block.BlockHeader.MerkleRoot,
                block.BlockHeader.TimeStamp,
                block.BlockHeader.Difficulty,
                block.BlockHeader.Validator,
                block.Height);

            return block.Hash == calculatedHash;
        }
    }
}
