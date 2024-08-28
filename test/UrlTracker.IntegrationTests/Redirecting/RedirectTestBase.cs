using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using UrlTracker.Core;
using UrlTracker.Core.Models;

namespace UrlTracker.IntegrationTests.Redirecting
{
    public class RedirectTestBase : IntegrationTestBase
    {
        protected const string _defaultTargetUrl = "https://example.com/";
        protected const HttpStatusCode _defaultRedirectCode = HttpStatusCode.Redirect;
        protected const bool _defaultPermanent = _defaultRedirectCode != HttpStatusCode.Redirect;
        protected IUmbracoContext UmbracoContext => ContextReference.UmbracoContext;
        private UmbracoContextReference ContextReference { get; set; } = null!;

        protected Redirect CreateRedirectBase()
        {
            return new Redirect
            {
                Force = false,
                RetainQuery = false,
                Permanent = _defaultPermanent,
                Target = new ContentPageTargetStrategy(GetDefaultRootNode(), "en-US")
            };
        }

        public override void Setup()
        {
            base.Setup();
            var umbracoContextFactory = ServiceProvider.GetRequiredService<IUmbracoContextFactory>();

            ContextReference = umbracoContextFactory.EnsureUmbracoContext();
        }

        public override void TearDown()
        {
            ContextReference.Dispose();
            base.TearDown();
        }
        protected IPublishedContent GetDefaultRootNode() => UmbracoContext.Content!.GetAtRoot().First();
        protected IRedirectService GetRedirectService() => ServiceProvider.GetRequiredService<IRedirectService>();
    }
}