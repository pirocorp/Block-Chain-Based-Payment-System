namespace PaymentSystem.Common.Extensions
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Microsoft.Extensions.DependencyInjection;

    using PaymentSystem.Common.Mapping;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAutoMapper(this IServiceCollection services, params Assembly[] assemblies)
        {
            AutoMapperConfig.RegisterMappings(assemblies); // Configuration

            services.AddSingleton(AutoMapperConfig.MapperInstance); // Register Service

            return services;
        }
    }
}
