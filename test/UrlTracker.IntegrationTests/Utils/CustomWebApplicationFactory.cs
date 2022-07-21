using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using ThrowawayDb;
using UrlTracker.Resources.Website;

namespace UrlTracker.IntegrationTests.Utils
{
    public class UrlTrackerWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly ThrowawayDatabase _database;
        public UrlTrackerWebApplicationFactory(ThrowawayDatabase database)
        {
            _database = database;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, "Integration.settings.json");
            builder.ConfigureAppConfiguration(conf =>
            {
                conf.AddJsonFile(configPath);
                conf.AddInMemoryCollection(new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>("ConnectionStrings:umbracoDbDSN", _database.ConnectionString)
                });
            });
        }

        public HttpClient CreateStandardClient()
            => CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                BaseAddress = new Uri("http://urltracker.ic")
            });
    }
}
