namespace PaymentSystem.BlockChain.Services.Hubs
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.SignalR;

    using PaymentSystem.Common.Hubs;
    using PaymentSystem.Common.Hubs.Models;

    public class BroadcastHub : Hub<IBroadcastClient>
    {
        public async Task SendBlock(NotificationBlock block)
            => await this.Clients.All.ReceiveBlock(block);
    }
}
