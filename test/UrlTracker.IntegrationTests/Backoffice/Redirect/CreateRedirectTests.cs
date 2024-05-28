using System.Net.Http.Headers;
using System.Net.Http.Json;
using UrlTracker.Backoffice.UI.Controllers.Models.Base;
using UrlTracker.Backoffice.UI.Controllers.Models.Redirects;
using UrlTracker.Core.Models;

namespace UrlTracker.IntegrationTests.Backoffice.Redirect
{
    public class CreateRedirectTests : RedirectTestBase
    {
        private const string _endpoint = _endpointBase;

        [TestCase(TestName = "Create creates a new redirect")]
        public async Task Create_DefaultFlow_CreatesRedirect()
        {
            // arrange
            var request = new RedirectRequest
            {
                Force = true,
                Permanent = true,
                RetainQuery = true,
                Source = new StrategyViewModel
                {
                    Strategy = Core.Defaults.DatabaseSchema.RedirectSourceStrategies.Url,
                    Value = "https://example.com/lorem/ipsum"
                },
                Target = new StrategyViewModel
                {
                    Strategy = Core.Defaults.DatabaseSchema.RedirectTargetStrategies.Content,
                    Value = GetDefaultRootNode().Id.ToString()
                }
            };

            var body = JsonContent.Create(request, MediaTypeHeaderValue.Parse("application/json"));

            // act
            var response = await WebsiteFactory.CreateStandardClient().PostAsync(_endpoint, body);
            response.EnsureSuccessStatusCode();
            var responseObject = await DeserializeResponseAsync<RedirectResponse>(response);

            // assert
            Assert.That(responseObject, Is.Not.Null);
            var createdRedirect = await GetRedirectService().GetAsync(responseObject.Id);
            Assert.That(createdRedirect, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(createdRedirect.Force, Is.EqualTo(request.Force));
                Assert.That(createdRedirect.Permanent, Is.EqualTo(request.Permanent));
                Assert.That(createdRedirect.RetainQuery, Is.EqualTo(request.RetainQuery));
                Assert.That(createdRedirect.Source, Is.InstanceOf<UrlSourceStrategy>());
                Assert.That(createdRedirect.Target, Is.InstanceOf<ContentPageTargetStrategy>());

                Assert.That(responseObject.Force, Is.EqualTo(request.Force));
                Assert.That(responseObject.Permanent, Is.EqualTo(request.Permanent));
                Assert.That(responseObject.RetainQuery, Is.EqualTo(request.RetainQuery));
                Assert.That(responseObject.Source, Is.EqualTo(request.Source));
                Assert.That(responseObject.Target, Is.EqualTo(request.Target));
            });
        }
    }
}
