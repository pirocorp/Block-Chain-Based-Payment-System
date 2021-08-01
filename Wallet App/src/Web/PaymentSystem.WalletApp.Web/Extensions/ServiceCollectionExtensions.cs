namespace PaymentSystem.WalletApp.Web.Extensions
{
    using System;
    using System.Linq;
    using System.Reflection;
    using CloudinaryDotNet;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Services;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCloudinary(this IServiceCollection services, IConfiguration configuration)
        {
            var account = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]);

            var cloudinary = new Cloudinary(account);

            services.AddSingleton(cloudinary);

            return services;
        }

        /// <summary>
        /// Add all services in IServiceCollection Container from the target Assembly
        /// </summary>
        /// <param name="services">IServiceCollection Container</param>
        /// <param name="type">Type from target Assembly</param>
        /// <returns>IServiceCollection Container with added services</returns>
        public static IServiceCollection AddDomainServices(this IServiceCollection services, Type type)
        {
            Assembly
                .GetAssembly(type)
                ?.GetTypes()
                .Where(t => t.IsClass
                            && t.GetInterfaces()
                                .Any(i => i.Name == $"I{t.Name}"))
                .Select(t => new
                {
                    Interface = t.GetInterface($"I{t.Name}"),
                    Implementation = t
                })
                .Where(s => s.Interface is not null)
                .ToList()
                .ForEach(s => services.AddTransient(s.Interface, s.Implementation));

            return services;
        }
    }
}
