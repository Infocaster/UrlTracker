using System.Threading.Tasks;
using NUnit.Framework;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Resources.Testing;
using UrlTracker.Resources.Testing.Logging;
using UrlTracker.Web.Processing;

namespace UrlTracker.Web.Tests.Processing
{
    public class ConfigurationInterceptFilterTests : TestBase
    {
        private ConfigurationInterceptFilter? _testSubject;

        public override void SetUp()
        {
            _testSubject = new ConfigurationInterceptFilter(UrlTrackerSettings!, new ConsoleLogger<ConfigurationInterceptFilter>());
        }

        [TestCase(true, false, TestName = "EvaluateCandidateAsync returns false if url tracker is disabled")]
        [TestCase(false, true, TestName = "EvaluateCandidateAsync returns true if url tracker is not disabled")]
        public async Task EvaluateCandidateAsync_NormalFlow_ReturnsResult(bool disabled, bool expected)
        {
            // arrange
            UrlTrackerSettings!.Value.IsDisabled = disabled;

            // act
            var result = await _testSubject!.EvaluateCandidateAsync(Url.Parse("http://example.com"));

            // assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
