using System.Threading.Tasks;
using NUnit.Framework;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Resources.Testing;
using UrlTracker.Web.Processing;

namespace UrlTracker.Web.Tests.Processing
{
    public class CoreConfigurationRequestInterceptFilterTests : TestBase
    {
        private CoreConfigurationRequestInterceptFilter _testSubject = null!;

        public override void SetUp()
        {
            _testSubject = new CoreConfigurationRequestInterceptFilter(UrlTrackerSettings);
        }

        [TestCase(true, true, TestName = "EvaluateCandidateAsync returns true if Enable is true")]
        [TestCase(false, false, TestName = "EvaluateCandidateAsync returns false if Enable is false")]
        public async Task EvaluateCandidate_Configuration_ReturnsCorrectResult(bool enable, bool expected)
        {
            // arrange
            var optionsValue = UrlTrackerSettings.CurrentValue;
            optionsValue.Enable = enable;

            // act
            var result = await _testSubject.EvaluateCandidateAsync(Url.Parse("http://example.com"));

            // assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
