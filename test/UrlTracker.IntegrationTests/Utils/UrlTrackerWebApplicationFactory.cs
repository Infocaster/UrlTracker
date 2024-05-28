using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Web.Common.Authorization;
using UrlTracker.Middleware.Background;
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

            builder.ConfigureServices(ConfigureServices);
        }

        private void ConfigureServices(IServiceCollection obj)
        {
            obj.AddSingleton<IAuthorizationHandler, TestAuthorizationHandler>();
            obj.AddAuthorization(options =>
            {
                options.AddPolicy(AuthorizationPolicies.BackOfficeAccess, policy =>
                {
                    policy.Requirements.Clear();
                    policy.AddRequirements(new TestRequirement());
                });
            });

            obj.RemoveAll(s => s.ServiceType == typeof(IClientErrorProcessorQueue));
            obj.AddSingleton<IClientErrorProcessorQueue, QueuelessClientErrorHandler>();


            var settings = new Umbraco.Cms.Infrastructure.PublishedCache.PublishedSnapshotServiceOptions
            {
                IgnoreLocalDb = true
            };
            obj.AddSingleton(settings);
        }

        public HttpClient CreateStandardClient()
        {
            HttpClient client = CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                BaseAddress = new Uri("http://localhost"),
            });

            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Chrome", "123.0.0.0"));
            return client;
        }

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
