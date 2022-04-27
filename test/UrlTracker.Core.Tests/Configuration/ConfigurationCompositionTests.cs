using Moq;
using NUnit.Framework;
using Umbraco.Core.Composing;
using UrlTracker.Core.Configuration;
using UrlTracker.Resources.Testing;
using UrlTracker.Resources.Testing.Objects;

namespace UrlTracker.Core.Tests.Configuration
{
    public class ConfigurationCompositionTests : TestBase
    {
        [TestCase(TestName = "Create registers configuration provider type")]
        public void Create_NormalFlow_RegistersConfigurationProviderType()
        {
            // arrange

            // act
            var result = RegisterMock.Object.Configure<object, TestConfiguration<object>>();

            // assert
            RegisterMock.Mock.Verify(obj => obj.Register(typeof(IConfiguration<object>), typeof(TestConfiguration<object>), Lifetime.Transient), Times.Once);
        }

        [TestCase(TestName = "Lazy decorates configuration with lazy decorator")]
        public void Lazy_NormalFlow_DecoratesConfigurationWithLazy()
        {
            // arrange
            var composition = RegisterMock.Object.Configure<object, TestConfiguration<object>>();
            RegisterMock.ServiceRegistryMock.Setup(obj => obj.Decorate<IConfiguration<object>, LazyConfigurationProvider<object>>())
                                            .Returns(RegisterMock.ServiceRegistryMock.Object)
                                            .Verifiable();

            // act
            composition.Lazy();

            // assert
            RegisterMock.ServiceRegistryMock.Verify();
        }
    }
}
