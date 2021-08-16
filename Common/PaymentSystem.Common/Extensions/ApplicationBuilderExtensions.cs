namespace PaymentSystem.Common.Extensions
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder ApplyMigrations<T>(this IApplicationBuilder builder)
            where T : DbContext
        {
            using var serviceScope = builder.ApplicationServices.CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<T>();

            if (dbContext.Database.IsRelational())
            {
                dbContext.Database.Migrate();
            }

            return builder;
        }
    }
}
