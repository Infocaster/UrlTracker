using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using UrlTracker.Core.Models;
using UrlTracker.Middleware.Options;
using UrlTracker.Middleware.Processing;

namespace UrlTracker.Middleware.Tests.Processing
{
    public class PipelineConfigurationRequestFilterTests
    {
        private PipelineConfigurationRequestInterceptFilter _testSubject = null!;
        private Mock<IOptionsMonitor<UrlTrackerPipelineOptions>> _urlTrackerPipelineOptionsMock;

        [SetUp]
        public void SetUp()
        {
            _urlTrackerPipelineOptionsMock = new Mock<IOptionsMonitor<UrlTrackerPipelineOptions>>();
            _urlTrackerPipelineOptionsMock.SetupGet(obj => obj.CurrentValue).Returns(new UrlTrackerPipelineOptions());
            _testSubject = new PipelineConfigurationRequestInterceptFilter(_urlTrackerPipelineOptionsMock.Object);
        }

        [TestCase(true, true, TestName = "EvaluateCandidateAsync returns true if Enable is true")]
        [TestCase(false, false, TestName = "EvaluateCandidateAsync returns false if Enable is false")]
        public async Task EvaluateCandidate_Configuration_ReturnsCorrectResult(bool enable, bool expected)
        {
            // arrange
            var optionsValue = _urlTrackerPipelineOptionsMock.Object.CurrentValue;
            optionsValue.Enable = enable;

            // act
            var result = await _testSubject.EvaluateCandidateAsync(Url.Parse("http://example.com"));

            // assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
