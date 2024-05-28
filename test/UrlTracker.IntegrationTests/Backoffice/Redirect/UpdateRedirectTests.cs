using System.Net.Http.Json;
using UrlTracker.Backoffice.UI.Controllers.Models.Base;
using UrlTracker.Backoffice.UI.Controllers.Models.Redirects;
using UrlTracker.Core.Models;

namespace UrlTracker.IntegrationTests.Backoffice.Redirect
{
    public class UpdateRedirectTests : RedirectTestBase
    {
        private const string _endpoint = _endpointBase;

        [TestCase(TestName = "Update updates redirects")]
        public async Task Update_NormalFlow_UpdatesRedirect()
        {
            // arrange
            var model = await CreateStandardRedirectAsync();
            var request = new RedirectRequest
            {
                Force = true,
                Permanent = false,
                RetainQuery = false,
                Source = new StrategyViewModel
                {
                    Strategy = Core.Defaults.DatabaseSchema.RedirectSourceStrategies.RegularExpression,
                    Value = @"^(lorem)$"
                },
                Target = new StrategyViewModel
                {
                    Strategy = Core.Defaults.DatabaseSchema.RedirectTargetStrategies.Url,
                    Value = "https://example.com"
                }
            };

            // act
            var response = await WebsiteFactory.CreateStandardClient().PostAsync(_endpoint + "/" + model.Id, JsonContent.Create(request));
            response.EnsureSuccessStatusCode();
            var responseModel = await DeserializeResponseAsync<RedirectResponse>(response);
            var redirect = await GetRedirectService().GetAsync(model.Id!.Value);

            // assert
            Assert.That(redirect, Is.Not.Null);
            Assert.That(responseModel, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(redirect.Force, Is.EqualTo(request.Force));
                Assert.That(redirect.Permanent, Is.EqualTo(request.Permanent));
                Assert.That(redirect.RetainQuery, Is.EqualTo(request.RetainQuery));
                Assert.That(redirect.Source, Is.InstanceOf<RegexSourceStrategy>());
                Assert.That(redirect.Target, Is.InstanceOf<UrlTargetStrategy>());

                Assert.That(responseModel.Force, Is.EqualTo(request.Force));
                Assert.That(responseModel.Permanent, Is.EqualTo(request.Permanent));
                Assert.That(responseModel.RetainQuery, Is.EqualTo(request.RetainQuery));
                Assert.That(responseModel.Source, Is.EqualTo(request.Source));
                Assert.That(responseModel.Target, Is.EqualTo(request.Target));
            });
        }
    }
}
