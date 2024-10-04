using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using UrlTracker.Core.Models;

namespace UrlTracker.IntegrationTests.Redirecting
{
    public class ForceTests : RedirectTestBase
    {
        private string DefaultSourceUrl
            => GetDefaultRootNode().FirstChild(ServiceProvider.GetRequiredService<IVariationContextAccessor>())!
                                   .Url(ServiceProvider.GetRequiredService<IPublishedUrlProvider>(), mode: UrlMode.Absolute);

        private Redirect CreateRedirect(bool force, string sourceUrl)
        {
            var redirect = CreateRedirectBase();
            redirect.Force = force;
            redirect.Target = new UrlTargetStrategy(_defaultTargetUrl);
            redirect.Source = new UrlSourceStrategy(sourceUrl);

            return redirect;
        }

        [TestCase(true, _defaultRedirectCode, TestName = "Request redirects from existing content if force is enabled")]
        [TestCase(false, HttpStatusCode.OK, TestName = "Request serves existing content if force is disabled")]
        public async Task Force(bool force, HttpStatusCode statusCode)
        {
            // arrange
            string defaultSourceUrl = DefaultSourceUrl;
            await GetRedirectService().AddAsync(CreateRedirect(force, defaultSourceUrl));

            // act
            var response = await WebsiteFactory.CreateStandardClient().GetAsync(defaultSourceUrl);

            // assert
            Assert.That(response.StatusCode, Is.EqualTo(statusCode));
        }

        [TestCase(TestName = "Forced redirects take precedence over non-forced redirects")]
        public async Task Force_MultipleMatches_TakeForcedRedirect()
        {
            // arrange
            string defaultSourceUrl = DefaultSourceUrl;
            var firstRedirect = CreateRedirect(false, defaultSourceUrl);
            firstRedirect.Target = new UrlTargetStrategy("http://urltracker.ic");
            await GetRedirectService().AddAsync(firstRedirect);
            await GetRedirectService().AddAsync(CreateRedirect(true, defaultSourceUrl));

            // act
            var response = await WebsiteFactory.CreateStandardClient().GetAsync(defaultSourceUrl);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(_defaultRedirectCode));
                Assert.That(response.Headers.Location, Is.EqualTo(new Uri(_defaultTargetUrl)));
            });
        }
    }
}
