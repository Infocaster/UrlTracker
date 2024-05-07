using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using UrlTracker.Core;

namespace UrlTracker.IntegrationTests.RecommendationTracking
{
    public class RecommendationTrackingTests : IntegrationTestBase
    {
        [TestCase(TestName = "A recommendation is created when a url is not found")]
        public async Task CreateRecommendation_NormalFlow_CreatesProperRecommendation()
        {
            // arrange

            // act
            var response = await WebsiteFactory.CreateStandardClient().GetAsync("/image.jpeg");

            // assert
            var recommendationService = ServiceProvider.GetRequiredService<IRecommendationService>();
            var recommendation = recommendationService.Get("http://localhost/image.jpeg/", Defaults.DatabaseSchema.RedactionScores.Media);
            
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(recommendation, Is.Not.Null);
                Assert.That(recommendation!.VariableScore, Is.EqualTo(1));
            });
        }

        [TestCase(TestName = "A recommendation is incremented when a url is not found multiple times")]
        public async Task IncrementRecommendation_NormalFlow_IncrementsRecommendation()
        {
            // arrange

            // act
            var client = WebsiteFactory.CreateStandardClient();
            var response = await client.GetAsync("/image.jpeg");
            var response2 = await client.GetAsync("/image.jpeg");

            // assert
            var recommendationService = ServiceProvider.GetRequiredService<IRecommendationService>();
            var recommendation = recommendationService.Get("http://localhost/image.jpeg/", Defaults.DatabaseSchema.RedactionScores.Media);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(response2.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(recommendation, Is.Not.Null);
                Assert.That(recommendation!.VariableScore, Is.EqualTo(2));
            });
        }
    }
}
