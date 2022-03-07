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
            ScopeMock = new Mock<IScope>();

            ProviderMock.Setup(obj => obj.CreateScope(It.IsAny<IsolationLevel>(),
                                                      It.IsAny<RepositoryCacheMode>(),
                                                      It.IsAny<IEventDispatcher>(),
                                                      It.IsAny<IScopedNotificationPublisher>(),
                                                      It.IsAny<bool?>(),
                                                      It.IsAny<bool>(),
                                                      It.IsAny<bool>()))
                        .Returns(ScopeMock.Object);
        }

        public Mock<IScopeProvider> ProviderMock { get; }
        public Mock<IScope> ScopeMock { get; }

        public IScopeProvider Provider => ProviderMock.Object;
        public IScope Scope => ScopeMock.Object;
    }
}
