using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using UrlTracker.Core.Models;

namespace UrlTracker.IntegrationTests.Redirecting
{
    public class RedirectFromTests : RedirectTestBase
    {
        protected Redirect CreateRedirectFromBase()
        {
            var result = CreateRedirectBase();

            result.Target = new UrlTargetStrategy(_defaultTargetUrl);
            return result;
        }

        protected Redirect CreateRedirectFromUrl(string url)
        {
            var redirect = CreateRedirectFromBase();
            redirect.Source = new UrlSourceStrategy(url);
            return redirect;
        }

        protected Redirect CreateRedirectFromRegex(string regex)
        {
            var redirect = CreateRedirectFromBase();
            redirect.Source = new RegexSourceStrategy(regex);
            return redirect;
        }

        protected static void AssertDefaultRedirectTarget(HttpResponseMessage? response)
        {
            Assert.That(response, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(_defaultRedirectCode));
                Assert.That(response.Headers.Location, Is.EqualTo(new Uri(_defaultTargetUrl)));
            });
        }

        [TestCase(TestName = "Source url can be relative")]
        public async Task Redirect_RelativeRedirect_PerformsRedirect()
        {
            // arrange
            const string sourceUrl = "/dolor/sit";
            await GetRedirectService().AddAsync(CreateRedirectFromUrl(sourceUrl));
            var client = WebsiteFactory.CreateStandardClient();

            // act
            var response = await client.GetAsync(sourceUrl);

            // assert
            AssertDefaultRedirectTarget(response);
        }

        [TestCase(TestName = "Source url can be absolute")]
        public async Task Redirect_AbsoluteRedirect_PerformsRedirect()
        {
            // arrange
            const string sourceUrl = "http://urltracker.ic/dolor/sit";
            await GetRedirectService().AddAsync(CreateRedirectFromUrl(sourceUrl));
            var client = WebsiteFactory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                BaseAddress = new Uri("http://urltracker.ic")
            });

            // act
            var response = await client.GetAsync(sourceUrl);

            // assert
            AssertDefaultRedirectTarget(response);
        }

        [TestCase(TestName = "Source can be regex")]
        public async Task Redirect_RegexRedirect_PerformsRedirect()
        {
            // arrange
            await GetRedirectService().AddAsync(CreateRedirectFromRegex(@"^[0-9]{6}\?lorem=(\w+)"));
            var client = WebsiteFactory.CreateStandardClient();

            // act
            var response = await client.GetAsync("/123456?lorem=ipsum");

            // assert
            AssertDefaultRedirectTarget(response);
        }

        [TestCase(TestName = "Source can have a query string")]
        public async Task Redirect_QueryStringDoesNotMatch_DoesNotRedirect()
        {
            // arrange
            await GetRedirectService().AddAsync(CreateRedirectFromUrl("/dolor/sit?lorem=ipsum"));
            var client = WebsiteFactory.CreateStandardClient();

            // act
            var response = await client.GetAsync("/dolor/sit?amet=conseptitur");

            // assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }
}
