using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using UrlTracker.Backoffice.UI.Controllers.Models.Recommendations;
using UrlTracker.Core;

namespace UrlTracker.IntegrationTests.Backoffice.Recommendation
{
    public class ListRecommendationsTests : RecommendationTestBase
    {
        private const string _endpoint = _endpointBase + "/list";

        [TestCase(TestName = "List returns a list of recommendations in the correct order")]
        public async Task List_NormalFlow_ReturnsCorrectResult()
        {
            // arrange
            var recommendationService = GetRecommendationService();

            var rec1 = recommendationService.CreateAndSave("https://example.com/lorem", Defaults.DatabaseSchema.RedactionScores.Page);
            var rec2 = recommendationService.CreateAndSave("https://urltracker.ic/ipsum", Defaults.DatabaseSchema.RedactionScores.Media);

            // act
            var result = await WebsiteFactory.CreateStandardClient().GetAsync(_endpoint + "?page=1&pageSize=10");
            result.EnsureSuccessStatusCode();
            var resultModel = await DeserializeResponseAsync<RecommendationCollectionResponse>(result);

            // assert
            Assert.That(resultModel, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(resultModel!.Total, Is.EqualTo(2));
                Assert.That(resultModel.Results.First().Id, Is.EqualTo(rec1.Id));
            });
        }
    }
}
