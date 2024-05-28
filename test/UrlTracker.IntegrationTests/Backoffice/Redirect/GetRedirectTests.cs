using UrlTracker.Backoffice.UI.Controllers.Models.Base;
using UrlTracker.Backoffice.UI.Controllers.Models.Redirects;

namespace UrlTracker.IntegrationTests.Backoffice.Redirect
{
    public class GetRedirectTests : RedirectTestBase
    {
        [TestCase(TestName = "Get returns 200OK with the inserted redirect")]
        public async Task Get_NormalFlow_ReturnsRedirect()
        {
            Core.Models.Redirect model = await CreateStandardRedirectAsync();
            var expected = new RedirectResponse
            {
                CreateDate = model.Inserted,
                Id = model.Id!.Value,
                Force = false,
                Key = model.Key!.Value,
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
            var response = await WebsiteFactory.CreateStandardClient().GetAsync(_endpointGet + "/" + model.Id);
            response.EnsureSuccessStatusCode();
            var responseModel = await DeserializeResponseAsync<RedirectResponse>(response);

            // assert
            Assert.That(responseModel, Is.EqualTo(expected));
        }

        [TestCase(TestName = "List returns a list of redirects")]
        public async Task List_NormalFlow_ReturnsRedirects()
        {
            // arrange
            var model = await CreateStandardRedirectAsync();

            // act
            var response = await WebsiteFactory.CreateStandardClient().GetAsync(_endpointList + "?page=1&pagesize=10");
            response.EnsureSuccessStatusCode();
            var responseModel = await DeserializeResponseAsync<RedirectCollectionResponse>(response);

            // assert
            Assert.That(responseModel, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(responseModel.Total, Is.EqualTo(1));
                Assert.That(responseModel.Results.Count, Is.EqualTo(1));
            });
        }

        private const string _endpointGet = _endpointBase;
        private const string _endpointList = _endpointBase;
    }
}
