using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using UrlTracker.Core.Abstractions;
using UrlTracker.Core.Models;

namespace UrlTracker.IntegrationTests.Redirecting
{
    public class RedirectToTests : RedirectTestBase
    {
        protected Redirect CreateRedirectToBase()
        {
            var result = CreateRedirectBase();
            result.Source = new UrlSourceStrategy("/dolor/sit");

            return result;
        }

        protected Redirect CreateRedirectToUrl(string url)
        {
            var result = CreateRedirectToBase();
            result.Target = new UrlTargetStrategy(url);

            return result;
        }

        protected Redirect CreateRedirectToContent(IPublishedContent content)
        {
            var result = CreateRedirectToBase();
            result.Target = new ContentPageTargetStrategy(content, null);

            return result;
        }

        protected Redirect CreateRedirectWithCulture(IPublishedContent content, string culture)
        {
            var result = CreateRedirectToBase();
            result.Target = new ContentPageTargetStrategy(content, culture);

            return result;
        }

        protected Task<HttpResponseMessage> RequestDefaultUrlAsync(HttpClient? client = null)
        {
            return (client ?? WebsiteFactory.CreateStandardClient()).GetAsync("/dolor/sit");
        }

        [TestCase(TestName = "Target url can be absolute")]
        public async Task Redirect_AbsoluteTarget_Redirects()
        {
            // arrange
            const string targetUrl = "https://example.com/lorem/";
            await GetRedirectService().AddAsync(CreateRedirectToUrl(targetUrl));

            // act
            var response = await RequestDefaultUrlAsync();

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(_defaultRedirectCode));
                Assert.That(response.Headers.Location, Is.EqualTo(new Uri(targetUrl)));
            });
        }

        [TestCase(TestName = "Target url can be relative")]
        public async Task Redirect_RelativeTarget_Redirects()
        {
            // arrange
            const string targetPath = "/lorem/";
            await GetRedirectService().AddAsync(CreateRedirectToUrl(targetPath));
            HttpClient client = WebsiteFactory.CreateStandardClient();

            // act
            var response = await RequestDefaultUrlAsync(client);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(_defaultRedirectCode));
                Assert.That(response.Headers.Location, Is.EqualTo(new Uri(client.BaseAddress!, targetPath)));
            });
        }

        [TestCase(TestName = "Target can be content")]
        public async Task Redirect_ContentTarget_Redirects()
        {
            // arrange
            var targetContent = GetDefaultRootNode().FirstChild(ServiceProvider.GetRequiredService<IVariationContextAccessor>())!;
            await GetRedirectService().AddAsync(CreateRedirectToContent(targetContent));
            var urlProvider = ServiceProvider.GetRequiredService<IPublishedUrlProvider>();
            var client = WebsiteFactory.CreateStandardClient();

            // act
            var response = await RequestDefaultUrlAsync(client);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(_defaultRedirectCode));
                Assert.That(response.Headers.Location, Is.EqualTo(new Uri(targetContent.Url(urlProvider, culture: null, mode: UrlMode.Absolute))));
            });
        }

        [TestCase(TestName = "Target can contain regex capture groups")]
        public async Task Redirect_RegexCaptureGroupTarget_Redirects()
        {
            // arrange
            var redirect = CreateRedirectBase();
            redirect.Source = new RegexSourceStrategy(@"^[0-9]{6}\?lorem=(\w+)");
            redirect.Target = new UrlTargetStrategy("/$1");
            await GetRedirectService().AddAsync(redirect);
            var client = WebsiteFactory.CreateStandardClient();
            const string targetPath = "ipsum";

            // act
            var response = await client.GetAsync("/123456?lorem=" + targetPath);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(_defaultRedirectCode));
                Assert.That(response.Headers.Location, Is.EqualTo(new Uri(client.BaseAddress!, targetPath + '/')));
            });
        }

        [TestCase(TestName = "Request returns http 410 if target node no longer exists")]
        public async Task Redirect_TargetNodeNoLongerExists_ReturnsGone()
        {
            // arrange
            var targetContent = GetDefaultRootNode().FirstChild(ServiceProvider.GetRequiredService<IVariationContextAccessor>())!;
            await GetRedirectService().AddAsync(CreateRedirectToContent(targetContent));

            var contentService = ServiceProvider.GetRequiredService<IContentService>();
            var targetEntity = contentService.GetById(targetContent.Id)!;
            contentService.Delete(targetEntity);

            var client = WebsiteFactory.CreateStandardClient();

            // act
            var response = await RequestDefaultUrlAsync(client);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Gone));
            });
        }

        [TestCase(TestName = "Culture can be uppercase")]
        public async Task Redirect_CultureUpperCase_ReturnsGone()
        {
            // arrange
            var targetContent = GetDefaultRootNode().FirstChild(ServiceProvider.GetRequiredService<IVariationContextAccessor>())!;
            await GetRedirectService().AddAsync(CreateRedirectWithCulture(targetContent ,"EN-US"));

            // act
            var response = await RequestDefaultUrlAsync();

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(_defaultRedirectCode));
            });
        }

        [TestCase(TestName = "Culture can be lowercase")]
        public async Task Redirect_CultureLowerCase_ReturnsGone()
        {
            // arrange
            var targetContent = GetDefaultRootNode().FirstChild(ServiceProvider.GetRequiredService<IVariationContextAccessor>())!;
            await GetRedirectService().AddAsync(CreateRedirectWithCulture(targetContent, "en-us"));

            // act
            var response = await RequestDefaultUrlAsync();

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(_defaultRedirectCode));
            });
        }
    }
}
