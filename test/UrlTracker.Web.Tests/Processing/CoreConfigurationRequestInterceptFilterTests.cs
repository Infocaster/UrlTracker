using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Core.Models;
using UrlTracker.Web.Processing.Filtering;

namespace UrlTracker.Web.Tests.Processing
{
    public class CoreConfigurationRequestInterceptFilterTests
    {
        private CoreConfigurationRequestInterceptFilter _testSubject = null!;
        private Mock<IOptionsMonitor<UrlTrackerSettings>> _urlTrackerSettingsMock;

        [SetUp]
        public void SetUp()
        {
            _urlTrackerSettingsMock = new Mock<IOptionsMonitor<UrlTrackerSettings>>();
            _urlTrackerSettingsMock.SetupGet(obj => obj.CurrentValue).Returns(new UrlTrackerSettings());
            _testSubject = new CoreConfigurationRequestInterceptFilter(_urlTrackerSettingsMock.Object);
        }

        [TestCase(true, true, TestName = "EvaluateCandidateAsync returns true if Enable is true")]
        [TestCase(false, false, TestName = "EvaluateCandidateAsync returns false if Enable is false")]
        public async Task EvaluateCandidate_Configuration_ReturnsCorrectResult(bool enable, bool expected)
        {
            // arrange
            var optionsValue = _urlTrackerSettingsMock.Object.CurrentValue;
            optionsValue.Enable = enable;

            // act
            var result = await _testSubject.EvaluateCandidateAsync(Url.Parse("http://example.com"));

            // assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
