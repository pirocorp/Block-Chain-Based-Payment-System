namespace PaymentSystem.BlockChain.Web.Infrastructure
{
    using System.Reflection;

    using Microsoft.Extensions.DependencyInjection;

    using PaymentSystem.BlockChain.Services.Mapping;
    using PaymentSystem.Common.Hubs.Models;

    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add AutoMapper in IServiceCollection (DI Container).
        /// </summary>
        /// <param name="services">IServiceCollection (DI Container).</param>
        /// <returns>IServiceCollection (DI Container) with added AutoMapper.</returns>
        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            AutoMapperConfig.RegisterMappings(
                typeof(NotificationBlock).GetTypeInfo().Assembly); // Configuration

            services.AddSingleton(AutoMapperConfig.MapperInstance); // Register Service

            return services;
        }
    }
}
