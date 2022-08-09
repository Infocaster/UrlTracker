using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using ThrowawayDb;
using UrlTracker.IntegrationTests.Utils;

namespace UrlTracker.IntegrationTests
{
    public class IntegrationTestBase
    {
        protected ThrowawayDatabase Database { get; private set; }
        public SnapshotScope SnapshotScope { get; private set; }
        protected UrlTrackerWebApplicationFactory WebsiteFactory { get; private set; }
        protected AsyncServiceScope Scope { get; private set; }
        protected IServiceProvider ServiceProvider => Scope.ServiceProvider;

        //[OneTimeSetUp]
        public virtual void OneTimeSetup()
        {
            Database = ThrowawayDatabase.FromLocalInstance(@"(LocalDb)\MSSQLLocalDB");

            // Install Umbraco on the temporary database
            using var factory = CreateApplicationFactory(Database);
            var client = factory.CreateClient();
            var policy = HttpPolicyExtensions.HandleTransientHttpError().RetryAsync(3);
            policy.ExecuteAsync(() => client.GetAsync("/")).Wait();
        }

        protected virtual UrlTrackerWebApplicationFactory CreateApplicationFactory(ThrowawayDatabase database)
        {
            return new UrlTrackerWebApplicationFactory(database);
        }

        [SetUp]
        public virtual void Setup()
        {
            // create database snapshot to return to after each test
            // Disabled, because it is bugged on the build server: https://github.com/Zaid-Ajaj/ThrowawayDb/issues/18
            //SnapshotScope = Database.CreateSnapshotScope();
            OneTimeSetup();
            WebsiteFactory = CreateApplicationFactory(Database);
            Scope = WebsiteFactory.Services.GetRequiredService<IServiceScopeFactory>().CreateAsyncScope();
        }

        [TearDown]
        public virtual void TearDown()
        {
            Scope.Dispose();
            WebsiteFactory.Dispose();

            // Disabled, because it is bugged on the build server: https://github.com/Zaid-Ajaj/ThrowawayDb/issues/18
            //SnapshotScope.Dispose();
            OneTimeTeardown();
        }

        //[OneTimeTearDown]
        public virtual void OneTimeTeardown()
        {
            Database.Dispose();
        }
    }
}
