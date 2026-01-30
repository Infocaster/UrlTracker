using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using UrlTracker.Core;
using UrlTracker.Core.Abstractions;
using UrlTracker.Core.Models;

namespace UrlTracker.IntegrationTests.Redirecting
{
    public class RedirectTestBase : IntegrationTestBase
    {
        protected const string _defaultTargetUrl = "https://example.com/";
        protected const HttpStatusCode _defaultRedirectCode = HttpStatusCode.Redirect;
        protected const bool _defaultPermanent = _defaultRedirectCode != HttpStatusCode.Redirect;
        protected IUmbracoContext UmbracoContext => ServiceProvider.GetRequiredService<IUmbracoContextAccessor>().GetRequiredUmbracoContext();

        protected Redirect CreateRedirectBase()
        {
            using var umbracoContext = ServiceProvider.GetRequiredService<IUmbracoContextFactoryAbstraction>().EnsureUmbracoContext();

            return new Redirect
            {
                Force = false,
                RetainQuery = false,
                Permanent = _defaultPermanent,
                Target = new ContentPageTargetStrategy(GetDefaultRootNode(umbracoContext), "en-US")
            };
        }

        protected IPublishedContent GetDefaultRootNode(IUmbracoContextReferenceAbstraction umbracoContext) => umbracoContext.GetContentAtRoot().First();
        protected IRedirectService GetRedirectService() => ServiceProvider.GetRequiredService<IRedirectService>();
    }
}