namespace PaymentSystem.BlockChain.Web.Extensions
{
    using System;
    using System.Linq;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    using PaymentSystem.BlockChain.Data;
    using PaymentSystem.BlockChain.Data.Seeding;
    using PaymentSystem.BlockChain.Services.Data;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder ApplyMigrations(this IApplicationBuilder builder)
        {
            using var serviceScope = builder.ApplicationServices.CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            dbContext.Database.Migrate();

            return builder;
        }

        public static IApplicationBuilder SeedData(this IApplicationBuilder builder)
        {
            using var serviceScope = builder.ApplicationServices.CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            new ApplicationDbContextSeeder().SeedAsync(dbContext, serviceScope.ServiceProvider).GetAwaiter().GetResult();

            return builder;
        }

        public static IApplicationBuilder UseSystemKeys(this IApplicationBuilder builder)
        {
            using var serviceScope = builder.ApplicationServices.CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var secret = dbContext.Settings.First(s => s.Key == nameof(SystemKeys.Secret)).Value;

            var systemKeys = builder.ApplicationServices.GetService<SystemKeys>();

            if (systemKeys is null)
            {
                throw new NullReferenceException($"In order to use SystemKeys they must be registered in IServiceCollection container.");
            }

            systemKeys.Update(secret);

            return builder;
        }
    }
}
