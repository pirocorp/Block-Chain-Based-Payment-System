namespace PaymentSystem.WalletApp.Tests.Mocks
{
    using System.Reflection;
    using AngleSharp.Text;
    using AutoMapper;
    using AutoMapper.Configuration;
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

        public static void Load<TSource, TDestination>()
        {
            Load();

            var config = new MapperConfigurationExpression();
            var maps = AutoMapperConfig.MapperInstance.ConfigurationProvider.GetAllTypeMaps();

            config.CreateProfile(
                "ReflectionProfile",
                configuration =>
                {
                    foreach (var typeMap in maps)
                    {
                        configuration.CreateMap(typeMap.SourceType, typeMap.DestinationType, typeMap.ConfiguredMemberList);
                    }

                    configuration.CreateMap<TSource, TDestination>();
                });

            AutoMapperConfig.MapperInstance = new Mapper(new MapperConfiguration(config));
        }
    }
}
