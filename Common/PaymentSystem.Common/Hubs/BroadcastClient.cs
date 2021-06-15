namespace PaymentSystem.Common.Hubs
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Hosting;
    using PaymentSystem.Common.Hubs.Models;

    // Example of consumer
    public abstract class BroadcastClient : IBroadcastClient, IHostedService
    {
        // private HubConnection _connection;

        public BroadcastClient()
        {
            //_connection = new HubConnectionBuilder()
            //    .WithUrl(Strings.HubUrl)
            //    .Build();
        }

        public Task ReceiveBlock(NotificationBlock block)
        {
            throw new System.NotImplementedException();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Loop is here to wait until the server is running
            while (true)
            {
                try
                {
                    // await _connection.StartAsync(cancellationToken);

                    break;
                }
                catch
                {
                    // await Task.Delay(1000);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // return _connection.DisposeAsync();
            throw new NotImplementedException();
        }
    }
}
