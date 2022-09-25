using System.Threading.Tasks;
using NUnit.Framework;
using UrlTracker.Middleware.Processing;
using UrlTracker.Resources.Testing;

namespace UrlTracker.Middleware.Tests.Processing
{
    public class PipelineConfigurationClientErrorFilterTests : TestBase
    {
        private PipelineConfigurationClientErrorFilter _testSubject = null!;

        public override void SetUp()
        {
            _testSubject = new PipelineConfigurationClientErrorFilter(UrlTrackerPipelineOptions);
        }

        [TestCase(true, true, true, TestName = "EvaluateCandidateAsync returns true if both Enable and EnableClientErrorTracking are true")]
        [TestCase(false, true, false, TestName = "EvaluateCandidateAsync returns false if Enable is false")]
        [TestCase(true, false, false, TestName = "EvaluateCandidateAsync returns false if EnableClientErrorTracking is false")]
        public async Task EvaluateCandidate_Configuration_ReturnsCorrectResult(bool enable, bool enableTracking, bool expected)
        {
            // arrange
            var optionsValue = UrlTrackerPipelineOptions.CurrentValue;
            optionsValue.Enable = enable;
            optionsValue.EnableClientErrorTracking = enableTracking;

            // act
            var result = await _testSubject.EvaluateCandidateAsync(HttpContextMock.Context);

            // assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
