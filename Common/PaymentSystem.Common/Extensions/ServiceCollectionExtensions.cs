namespace PaymentSystem.Common.Extensions
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Microsoft.Extensions.DependencyInjection;

    using PaymentSystem.Common.Mapping;

    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add AutoMapper in IServiceCollection (DI Container).
        /// </summary>
        /// <param name="services">IServiceCollection (DI Container).</param>
        /// <returns>IServiceCollection (DI Container) with added AutoMapper.</returns>
        public static IServiceCollection AddAutoMapper(this IServiceCollection services, params Type[] types)
        {
            var assemblies = types
                .Select(t => t.GetTypeInfo().Assembly)
                .ToArray();

            return AddAutoMapper(services, assemblies);
        }

        public static IServiceCollection AddAutoMapper(this IServiceCollection services, params Assembly[] assemblies)
        {
            AutoMapperConfig.RegisterMappings(assemblies); // Configuration

            services.AddSingleton(AutoMapperConfig.MapperInstance); // Register Service

            return services;
        }
    }
}
