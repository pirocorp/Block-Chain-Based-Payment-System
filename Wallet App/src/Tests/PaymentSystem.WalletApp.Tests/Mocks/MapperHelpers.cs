namespace PaymentSystem.WalletApp.Tests.Mocks
{
    using System.Reflection;

    using AutoMapper;
    using Moq;
    using PaymentSystem.Common.Mapping;
    using PaymentSystem.WalletApp.Services.Data.Models;
    using PaymentSystem.WalletApp.Web.ViewModels;

    public static class MapperHelpers
    {
        public static IMapper Instance => AutoMapperConfig.MapperInstance;

        public static IMapper Fake
        {
            get
            {
                var mapperMock = new Mock<IMapper>();

                mapperMock
                    .SetupGet(m => m.ConfigurationProvider)
                    .Returns(Mock.Of<IConfigurationProvider>());

                return mapperMock.Object;
            }
        }

        public static void Load()
        {
            AutoMapperConfig.RegisterMappings(
                typeof(ErrorViewModel).GetTypeInfo().Assembly,
                typeof(IServiceModel).GetTypeInfo().Assembly);
        }
    }
}
