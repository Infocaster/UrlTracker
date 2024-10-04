using Microsoft.Extensions.DependencyInjection;
using UrlTracker.IntegrationTests.Utils;

namespace UrlTracker.IntegrationTests
{
    public class IntegrationTestBase
    {
        protected UrlTrackerWebApplicationFactory WebsiteFactory { get; private set; }
        protected AsyncServiceScope Scope { get; private set; }
        protected IServiceProvider ServiceProvider => Scope.ServiceProvider;

        protected virtual UrlTrackerWebApplicationFactory CreateApplicationFactory()
        {
            return new UrlTrackerWebApplicationFactory();
        }

        [SetUp]
        public virtual void Setup()
        {
            WebsiteFactory = CreateApplicationFactory();
            Scope = WebsiteFactory.Services.GetRequiredService<IServiceScopeFactory>().CreateAsyncScope();
        }

        [TearDown]
        public virtual void TearDown()
        {
            Scope.Dispose();
            WebsiteFactory.Dispose();
        }
    }
}
