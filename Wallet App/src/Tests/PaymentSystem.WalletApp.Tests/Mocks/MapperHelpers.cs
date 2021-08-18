namespace PaymentSystem.WalletApp.Tests.Mocks
{
    using System.Reflection;

    using AutoMapper;

    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Services.Data.Models;
    using PaymentSystem.WalletApp.Web.ViewModels;

    public static class MapperHelpers
    {
        public static IMapper Instance => AutoMapperConfig.MapperInstance;

        public static void Load()
        {
            AutoMapperConfig.RegisterMappings(
                typeof(ErrorViewModel).GetTypeInfo().Assembly,
                typeof(IServiceModel).GetTypeInfo().Assembly);
        }
    }
}
