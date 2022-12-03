using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UrlTracker.Resources.Website;

namespace UrlTracker.IntegrationTests.Utils
{
    public class UrlTrackerWebApplicationFactory : WebApplicationFactory<Program>
    {
        private const string _inMemoryConnectionString = "Data Source=IntegrationTests;Mode=Memory;Cache=Shared";
        private readonly SqliteConnection _imConnection;

        public UrlTrackerWebApplicationFactory()
        {
            // In memory database only persists as long as there are active connections to it
            //    Therefore, keep one connection open while this web application factory is in use
            _imConnection = new SqliteConnection(_inMemoryConnectionString);
            _imConnection.Open();
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
                    new KeyValuePair<string, string>("ConnectionStrings:umbracoDbDSN", _inMemoryConnectionString),
                    new KeyValuePair<string, string>("ConnectionStrings:umbracoDbDSN_ProviderName", "Microsoft.Data.Sqlite")
                });
            });
        }

        public HttpClient CreateStandardClient()
            => CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                BaseAddress = new Uri("http://urltracker.ic")
            });

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            // When this application factory is disposed, close the connection to the in-memory database
            //    This will cause the database to be deleted
            _imConnection.Close();
            _imConnection.Dispose();
        }
    }
}
