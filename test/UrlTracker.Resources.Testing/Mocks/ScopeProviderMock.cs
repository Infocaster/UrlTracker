using System.Data;
using Moq;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Scoping;

namespace UrlTracker.Resources.Testing.Mocks
{
    public class ScopeProviderMock
    {
        public ScopeProviderMock()
        {
            ProviderMock = new Mock<IScopeProvider>();
            AccessorMock = new Mock<IScopeAccessor>();
            ScopeMock = new Mock<IScope>();

            ProviderMock.Setup(obj => obj.CreateScope(It.IsAny<IsolationLevel>(),
                                                      It.IsAny<Umbraco.Cms.Core.Scoping.RepositoryCacheMode>(),
                                                      It.IsAny<IEventDispatcher>(),
                                                      It.IsAny<IScopedNotificationPublisher>(),
                                                      It.IsAny<bool?>(),
                                                      It.IsAny<bool>(),
                                                      It.IsAny<bool>()))
                        .Returns(ScopeMock.Object);

            AccessorMock.SetupGet(obj => obj.AmbientScope)
                        .Returns(ScopeMock.Object);
        }

        public Mock<IScopeProvider> ProviderMock { get; }
        public Mock<IScopeAccessor> AccessorMock { get; }
        public Mock<IScope> ScopeMock { get; }

        public IScopeProvider Provider => ProviderMock.Object;
    }
}
