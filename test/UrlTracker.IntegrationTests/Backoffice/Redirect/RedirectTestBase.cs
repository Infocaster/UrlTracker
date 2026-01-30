using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using UrlTracker.Core;
using UrlTracker.Core.Abstractions;
using UrlTracker.Core.Models;

namespace UrlTracker.IntegrationTests.Backoffice.Redirect
{
    public class RedirectTestBase : BackofficeIntegrationTestBase
    {
        protected const string _endpointBase = "/umbraco/management/api/v1/UrlTracker/Redirects";

        public IUmbracoContextReferenceAbstraction ContextReference { get; private set; } = null!;

        public override async Task SetupAsync()
        {
            await base.SetupAsync();
            var umbracoContextFactory = ServiceProvider.GetRequiredService<IUmbracoContextFactoryAbstraction>();

            ContextReference = umbracoContextFactory.EnsureUmbracoContext();
        }

        public override void TearDown()
        {
            ContextReference.Dispose();
            base.TearDown();
        }

        protected IPublishedContent GetDefaultRootNode() => ContextReference.GetContentAtRoot().First();
        protected IRedirectService GetRedirectService() => ServiceProvider.GetRequiredService<IRedirectService>();


        protected async Task<Core.Models.Redirect> CreateStandardRedirectAsync()
        {
            // arrange
            return await GetRedirectService().AddAsync(new UrlTracker.Core.Models.Redirect
            {
                Force = false,
                Permanent = true,
                RetainQuery = true,
                Source = new UrlSourceStrategy("https://example.com/lorem/ipsum"),
                Target = new ContentPageTargetStrategy(GetDefaultRootNode(), null),
            });
        }
    }
}
