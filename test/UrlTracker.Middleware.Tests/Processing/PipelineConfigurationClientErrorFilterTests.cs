using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using UrlTracker.Middleware.Options;
using UrlTracker.Middleware.Processing;
using UrlTracker.Resources.Testing.Mocks;

namespace UrlTracker.Middleware.Tests.Processing
{
    public class PipelineConfigurationClientErrorFilterTests
    {
        private PipelineConfigurationClientErrorFilter _testSubject = null!;
        private Mock<IOptionsMonitor<UrlTrackerPipelineOptions>> _urlTrackerPipelineOptionsMock;

        [SetUp]
        public void SetUp()
        {
            _urlTrackerPipelineOptionsMock = new Mock<IOptionsMonitor<UrlTrackerPipelineOptions>>();
            _urlTrackerPipelineOptionsMock.SetupGet(obj => obj.CurrentValue).Returns(new UrlTrackerPipelineOptions());
            _testSubject = new PipelineConfigurationClientErrorFilter(_urlTrackerPipelineOptionsMock.Object);
        }

        [TestCase(true, true, true, TestName = "EvaluateCandidateAsync returns true if both Enable and EnableClientErrorTracking are true")]
        [TestCase(false, true, false, TestName = "EvaluateCandidateAsync returns false if Enable is false")]
        [TestCase(true, false, false, TestName = "EvaluateCandidateAsync returns false if EnableClientErrorTracking is false")]
        public async Task EvaluateCandidate_Configuration_ReturnsCorrectResult(bool enable, bool enableTracking, bool expected)
        {
            // arrange
            var optionsValue = _urlTrackerPipelineOptionsMock.Object.CurrentValue;
            optionsValue.Enable = enable;
            optionsValue.EnableClientErrorTracking = enableTracking;

            // act
            var result = await _testSubject.EvaluateCandidateAsync(new HttpContextMock().Context);

            // assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
