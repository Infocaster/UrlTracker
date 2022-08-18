using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Resources.Testing;
using UrlTracker.Resources.Testing.Logging;
using UrlTracker.Web.Processing;

namespace UrlTracker.Web.Tests.Processing
{
    public class ConfigurationInterceptFilterTests : TestBase
    {
        private ConfigurationInterceptFilter _testSubject;

        public override void SetUp()
        {
            _testSubject = new ConfigurationInterceptFilter(UrlTrackerSettings, new VoidLogger());
        }

        public static IEnumerable<TestCaseData> TestCases()
        {
            yield return new TestCaseData(true, false).SetName("EvaluateCandidateAsync returns false if url tracker is disabled");
            yield return new TestCaseData(false, true).SetName("EvaluateCandidateAsync returns true if url tracker is not disabled");
        }

        [TestCaseSource(nameof(TestCases))]
        public async Task EvaluateCandidateAsync_NormalFlow_ReturnsResult(bool disabled, bool expected)
        {
            // arrange
            UrlTrackerSettings.Value = new UrlTrackerSettings(disabled, default, default, default, default, default, 5000, true, 60 * 48, true, new List<string>());

            // act
            var result = await _testSubject.EvaluateCandidateAsync(Url.Parse("http://example.com"));

            // assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
