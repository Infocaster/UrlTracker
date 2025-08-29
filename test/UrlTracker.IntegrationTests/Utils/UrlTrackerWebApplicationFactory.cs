using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Cms.Web.Common.Authorization;
using UrlTracker.Middleware.Background;

namespace UrlTracker.IntegrationTests.Utils
{
    public class UrlTrackerWebApplicationFactory : WebApplicationFactory<Program>
    {
        private const string _inMemoryConnectionString = "Data Source=IntegrationTests;Mode=Memory;Cache=Shared";
        private readonly SqliteConnection _imConnection;
        private readonly SemaphoreSlim _startSemaphore = new(1, 1);
        private bool _started;

        public UrlTrackerWebApplicationFactory()
        {
            // In memory database only persists as long as there are active connections to it
            //    Therefore, keep one connection open while this web application factory is in use
            _imConnection = new SqliteConnection(_inMemoryConnectionString);
            _imConnection.Open();
        }

        public async Task StartAsync()
        {
            /* Websites are lazy loaded, so we need to prod the website to make it boot.
             * The wait handle is created before boot, so that we know for certain that boot hasn't finished before we start waiting (VERY IMPORTANT!!)
             * 
             * Sending a request to the server is the trigger that makes the website boot.
             * The wait handle will be triggered as soon as index rebuilding is complete. At that point, we know for sure that the website is completely ready.
             * 
             * Only one request can perform the start at a time, so we have to use the double-if pattern in combination with a semaphore to ensure that startup is really only called once.
             */
            if (!_started)
            {
                await _startSemaphore.WaitAsync();
                try
                {
                    if (!_started)
                    {
                        // The waitHandle task completes when the index rebuild is done.
                        ExamineWaitContext waitContext = Services.GetRequiredService<ExamineWaitContext>();
                        Task waitHandle = waitContext.SetAsync();

                        await CreateClient().GetAsync("/");
                        await waitHandle;

                        _started = true;
                    }
                }
                finally
                {
                    _startSemaphore.Release();
                }
            }
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, "Integration.settings.json");
            builder.ConfigureAppConfiguration(conf =>
            {
                conf.AddJsonFile(configPath);
                conf.AddInMemoryCollection(
                [
                    new("ConnectionStrings:umbracoDbDSN", _inMemoryConnectionString),
                    new("ConnectionStrings:umbracoDbDSN_ProviderName", "Microsoft.Data.Sqlite")
                ]);
            });

            builder.ConfigureServices(ConfigureServices);
        }

        private void ConfigureServices(IServiceCollection obj)
        {
            // In order to force consistent behaviour with indexes, we add a type that prevents the rebuild from running on a background thread.
            // Additionally, the "ExamineWaitContext" allows us to receive a signal when the index rebuild is complete.
            obj.Decorate<IIndexRebuilder, DecoratorIndexRebuilderNotifier>();
            obj.AddSingleton<ExamineWaitContext>();

            /* The umbraco rebuild on startup handler gets removed, because the static fields inside of it break tests when running multiple at once.
             *   If examine related tests start breaking, check if the implementation of this type has changed.
             * The service is replaced with a custom notification handler which uses a singleton class instead of static fields
             */
            ServiceDescriptor sd = obj.First(s => s.ImplementationType == typeof(RebuildOnStartupHandler));
            obj.Remove(sd);
            obj.Add(new UniqueServiceDescriptor(typeof(INotificationHandler<UmbracoRequestBeginNotification>), typeof(CustomRebuildOnStartupHandler), ServiceLifetime.Transient));
            obj.AddSingleton<CustomRebuildOnStartupHandlerState>();

            // Add a configuration to move the examine index files into RAM.
            // Now we don't rely on the filesystem while running tests
            obj.ConfigureOptions<ExamineInMemoryConfiguration>();

            // Supposedly, scheduled publishing does curses with locks which cause flaky tests with Sqlite.
            // This component disables scheduled publishing
            obj.AddHostedService<SuspendScheduledPublishingHostedService>();

            obj.AddSingleton<IAuthorizationHandler, TestAuthorizationHandler>();
            obj.AddAuthorizationBuilder()
                .AddPolicy(AuthorizationPolicies.BackOfficeAccess, policy =>
                {
                    policy.Requirements.Clear();
                    policy.AddRequirements(new TestRequirement());
                });

            obj.RemoveAll(s => s.ServiceType == typeof(IClientErrorProcessorQueue));
            obj.AddSingleton<IClientErrorProcessorQueue, QueuelessClientErrorHandler>();
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
