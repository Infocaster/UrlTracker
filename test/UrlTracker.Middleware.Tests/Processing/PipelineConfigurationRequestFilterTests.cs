using System.Threading.Tasks;
using NUnit.Framework;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Middleware.Processing;
using UrlTracker.Resources.Testing;

namespace UrlTracker.Middleware.Tests.Processing
{
    public class PipelineConfigurationRequestFilterTests : TestBase
    {
        private PipelineConfigurationRequestInterceptFilter _testSubject = null!;

        public override void SetUp()
        {
            _testSubject = new PipelineConfigurationRequestInterceptFilter(UrlTrackerPipelineOptions);
        }

        [TestCase(true, true, TestName = "EvaluateCandidateAsync returns true if Enable is true")]
        [TestCase(false, false, TestName = "EvaluateCandidateAsync returns false if Enable is false")]
        public async Task EvaluateCandidate_Configuration_ReturnsCorrectResult(bool enable, bool expected)
        {
            // arrange
            var optionsValue = UrlTrackerPipelineOptions.CurrentValue;
            optionsValue.Enable = enable;

            // act
            var result = await _testSubject.EvaluateCandidateAsync(Url.Parse("http://example.com"));

            // assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
