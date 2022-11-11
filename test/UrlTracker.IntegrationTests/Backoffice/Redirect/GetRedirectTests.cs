using UrlTracker.Backoffice.UI.Controllers.Models.Base;
using UrlTracker.Backoffice.UI.Controllers.Models.Redirects;

namespace UrlTracker.IntegrationTests.Backoffice.Redirect
{
    public class GetRedirectTests : RedirectTestBase
    {
        [TestCase(TestName = "Get returns 200OK with the inserted redirect")]
        public async Task TestGet()
        {
            Core.Models.Redirect model = await CreateStandardRedirectAsync();
            var expected = new RedirectResponse
            {
                CreateDate = model.Inserted,
                Id = model.Id!.Value,
                Force = false,
                Key = model.Key,
                Permanent = true,
                RetainQuery = true,
                Source = new StrategyViewModel
                {
                    Strategy = UrlTracker.Core.Defaults.DatabaseSchema.RedirectSourceStrategies.Url,
                    Value = "https://example.com/lorem/ipsum"
                },
                Target = new StrategyViewModel
                {
                    Strategy = UrlTracker.Core.Defaults.DatabaseSchema.RedirectTargetStrategies.Content,
                    Value = GetDefaultRootNode().Id.ToString()
                }
            };

            // act
            var response = await WebsiteFactory.CreateStandardClient().GetAsync(_endpoint + "/" + model.Id);
            response.EnsureSuccessStatusCode();
            var responseModel = await DeserializeResponseAsync<RedirectResponse>(response);

            // assert
            Assert.That(responseModel, Is.EqualTo(expected));
        }

        private const string _endpoint = _endpointBase + "/get";
    }
}
