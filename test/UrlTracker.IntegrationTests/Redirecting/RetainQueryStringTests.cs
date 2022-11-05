using UrlTracker.Core.Models;

namespace UrlTracker.IntegrationTests.Redirecting
{
    public class RetainQueryStringTests : RedirectTestBase
    {
        private const string _defaultSource = "/dolor/sit";
        private const string _defaultQuery = "?lorem=ipsum";

        protected Task AddRedirectAsync(bool retainQuery)
        {
            var redirect = CreateRedirectBase();
            redirect.Source = new UrlSourceStrategy(_defaultSource);
            redirect.Target = new UrlTargetStrategy(_defaultTargetUrl);
            redirect.RetainQuery = retainQuery;

            return GetRedirectService().AddAsync(redirect);
        }

        protected Task<HttpResponseMessage> RequestDefaultUrlAsync(HttpClient? client = null)
        {
            return (client ?? WebsiteFactory.CreateStandardClient()).GetAsync(_defaultSource + _defaultQuery);
        }

        [TestCase(TestName = "Query string is retained if setting is enabled")]
        public async Task RetainQuery_Enabled_RedirectContainsQuery()
        {
            // arrange
            await AddRedirectAsync(retainQuery: true);

            // act
            var response = await RequestDefaultUrlAsync();

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(_defaultRedirectCode));
                Assert.That(response.Headers.Location, Is.EqualTo(new Uri(new Uri(_defaultTargetUrl), _defaultQuery)));
            });
        }

        [TestCase(TestName = "Query string is truncated if setting is disabled")]
        public async Task RetainQuery_Disabled_RedirectDoesNotContainQuery()
        {
            // arrange
            await AddRedirectAsync(retainQuery: false);

            // act
            var response = await RequestDefaultUrlAsync();

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(_defaultRedirectCode));
                Assert.That(response.Headers.Location, Is.EqualTo(new Uri(_defaultTargetUrl)));
            });
        }
    }
}
