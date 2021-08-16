namespace PaymentSystem.WalletApp.Tests
{
    using System;
    using System.Linq;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    using PaymentSystem.WalletApp.Data;
    using PaymentSystem.WalletApp.Services.Data;
    using PaymentSystem.WalletApp.Tests.Mocks;

    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup>
            where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Configure in memory database.
                var dbContextDescriptor = services
                    .SingleOrDefault(d
                        => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                services.Remove(dbContextDescriptor);

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase(Guid.NewGuid().ToString());
                });

                // Replace SignalR Service
                var signalRServiceDescriptor = new ServiceDescriptor(
                    typeof(IBlockChainSignalRService),
                    BlockChainSignalRServiceMock.Instance
                );

                services.Replace(signalRServiceDescriptor);

                // Create in memory database.
                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<ApplicationDbContext>();

                db.Database.EnsureCreated();
            });
        }
    }
}
