using LightInject;
using Moq;
using Umbraco.Core.Composing;

namespace UrlTracker.Resources.Testing.Mocks
{
    public class RegisterMock
    {
        public RegisterMock()
        {
            Mock = new Mock<IRegister>();
            ServiceRegistryMock = new Mock<IServiceRegistry>();
            Mock.SetupGet(obj => obj.Concrete).Returns(ServiceRegistryMock.Object);
        }

        public Mock<IRegister> Mock { get; }
        public IRegister Object => Mock.Object;
        public Mock<IServiceRegistry> ServiceRegistryMock { get; }
    }
}
