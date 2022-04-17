using System;
using System.Threading.Tasks;
using NUnit.Framework;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Resources.Testing;
using UrlTracker.Resources.Testing.Mocks;
using UrlTracker.Web.Processing;

namespace UrlTracker.Web.Tests.Processing
{
    public class ConfigurationClientErrorFilterTests : TestBase
    {
        private ConfigurationClientErrorFilter _testSubject;

        public override void SetUp()
        {
            _testSubject = new ConfigurationClientErrorFilter(UrlTrackerSettings);
        }

        [TestCase(TestName = "EvaluateCandidateAsync returns true if the incoming request is a candidate")]
        public async Task EvaluateCandidateAsync_ValidCandidate_ReturnsTrue()
        {
            // arrange
            HttpContextMock = new HttpContextMock(new Uri("http://example.com/lorem"));
            UrlTrackerSettings.Value = new UrlTrackerSettings(false, false, true, false, false, false, 5000, true, 60 * 48, true);

            // act
            bool result = await _testSubject.EvaluateCandidateAsync(HttpContextMock.Context);

            // assert
            Assert.That(result, Is.True);
        }

        [TestCase(TestName = "EvaluateCandidateAsync returns false if the URL Tracker is disabled")]
        public async Task EvaluateCandidateAsync_PluginDisabled_ReturnsFalse()
        {
            // arrange
            UrlTrackerSettings.Value = new UrlTrackerSettings(true, false, true, false, false, false, 5000, true, 60 * 48, true);

            // act
            bool result = await _testSubject.EvaluateCandidateAsync(HttpContextMock.Context);

            // assert
            Assert.That(result, Is.False);
        }

        [TestCase(TestName = "EvaluateCandidateAsync returns false if not-found-tracking is disabled")]
        public async Task EvaluateCandidateAsync_TrackingDisabled_ReturnsFalse()
        {
            // arrange
            UrlTrackerSettings.Value = new UrlTrackerSettings(false, false, true, true, false, false, 5000, true, 60 * 48, true);

            // act
            bool result = await _testSubject.EvaluateCandidateAsync(HttpContextMock.Context);

            // assert
            Assert.That(result, Is.False);
        }

        [TestCase(TestName = "EvaluateCandidateAsync returns false if the incoming url is on the regex blacklist")]
        public async Task EvaluateCandidateAsync_CandidateBlacklisted_ReturnsFalse()
        {
            // arrange
            HttpContextMock = new HttpContextMock(new Uri("http://example.com/favicon.ico"));
            UrlTrackerSettings.Value = new UrlTrackerSettings(false, false, true, false, false, false, 5000, true, 60 * 48, true);

            // act
            bool result = await _testSubject.EvaluateCandidateAsync(HttpContextMock.Context);

            // assert
            Assert.That(result, Is.False);
        }
    }
}
