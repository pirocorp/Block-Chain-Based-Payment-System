namespace PaymentSystem.WalletApp.Services
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.SignalR.Client;

    using PaymentSystem.Common;
    using PaymentSystem.Common.Hubs;
    using PaymentSystem.Common.Hubs.Models;

    public class BlockChainSignalRService : IBlockChainSignalRService
    {
        private readonly IBlockService blockService;
        private readonly HubConnection connection;

        public BlockChainSignalRService(IBlockService blockService)
        {
            this.blockService = blockService;

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
                this.blockService.ReceiveBlock(notificationBlock)
                    .GetAwaiter()
                    .GetResult();
            });
        }
    }
}
