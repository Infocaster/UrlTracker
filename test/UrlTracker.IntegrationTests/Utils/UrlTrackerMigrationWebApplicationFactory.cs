using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ThrowawayDb;
using Umbraco.Cms.Infrastructure.Migrations;
using UrlTracker.Core.Database.Migrations;

namespace UrlTracker.IntegrationTests.Utils
{
    internal class UrlTrackerMigrationWebApplicationFactory : UrlTrackerWebApplicationFactory
    {
        public UrlTrackerMigrationWebApplicationFactory(ThrowawayDatabase database)
            : base(database)
        { }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureAppConfiguration(conf =>
            {
                conf.AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>(Core.Defaults.Options.UrlTrackerSection + ":IsDisabled", bool.TrueString)
                });
            });
            builder.ConfigureServices(collection =>
            {
                collection.AddUnique<IMigrationPlanFactory, EmptyMigrationPlanFactory>(ServiceLifetime.Singleton);
            });
        }

        private class EmptyMigrationPlanFactory : IMigrationPlanFactory
        {
            public MigrationPlan? Create()
            {
                return null;
            }
        }
    }
}
