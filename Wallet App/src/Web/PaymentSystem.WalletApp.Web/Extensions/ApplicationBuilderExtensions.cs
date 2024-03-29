﻿namespace PaymentSystem.WalletApp.Web.Extensions
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using PaymentSystem.WalletApp.Data;
    using PaymentSystem.WalletApp.Data.Seeding;
    using PaymentSystem.WalletApp.Services;
    using PaymentSystem.WalletApp.Services.Data;
    using PaymentSystem.WalletApp.Services.Data.Implementations;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder SeedData(this IApplicationBuilder builder)
        {
            using var serviceScope = builder.ApplicationServices.CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            new ApplicationDbContextSeeder()
                .SeedAsync(dbContext, serviceScope.ServiceProvider)
                .GetAwaiter()
                .GetResult();

            return builder;
        }

        public static IApplicationBuilder UseBlockNotifications(this IApplicationBuilder builder)
        {
            var signalRNotificationService =
                (IBlockChainSignalRService)builder.ApplicationServices.GetService(typeof(IBlockChainSignalRService));

            signalRNotificationService
                .Run()
                .GetAwaiter()
                .GetResult();

            return builder;
        }
    }
}
