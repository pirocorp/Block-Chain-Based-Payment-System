namespace PaymentSystem.BlockChain.Web.Extensions
{
    using Microsoft.Extensions.DependencyInjection;

    using PaymentSystem.BlockChain.Services.Data;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSystemKeys(this IServiceCollection services)
        {
            var keys = new SystemKeys();
            services.AddSingleton(keys);

            return services;
        }
    }
}
