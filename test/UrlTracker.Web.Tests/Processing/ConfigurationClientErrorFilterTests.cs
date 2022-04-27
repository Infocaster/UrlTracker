using System;
using System.Threading.Tasks;
using NUnit.Framework;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Resources.Testing;
using UrlTracker.Resources.Testing.Mocks;
using UrlTracker.Web.Events.Models;
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
            Uri url = new Uri("http://example.com/lorem");
            HttpContextMock = new HttpContextMock(url);
            UrlTrackerSettings.Value = new UrlTrackerSettings(false, false, true, false, false, false, 5000, true, 60 * 48, true);

            // act
            bool result = await _testSubject.EvaluateCandidateAsync(new ProcessedEventArgs(HttpContextMock.Context, Url.FromAbsoluteUri(url)));

            // assert
            Assert.That(result, Is.True);
        }

        [TestCase(TestName = "EvaluateCandidateAsync returns false if the URL Tracker is disabled")]
        public async Task EvaluateCandidateAsync_PluginDisabled_ReturnsFalse()
        {
            // arrange
            UrlTrackerSettings.Value = new UrlTrackerSettings(true, false, true, false, false, false, 5000, true, 60 * 48, true);

            // act
            bool result = await _testSubject.EvaluateCandidateAsync(new ProcessedEventArgs(HttpContextMock.Context, Url.Parse("http://example.com")));

            // assert
            Assert.That(result, Is.False);
        }

        [TestCase(TestName = "EvaluateCandidateAsync returns false if not-found-tracking is disabled")]
        public async Task EvaluateCandidateAsync_TrackingDisabled_ReturnsFalse()
        {
            // arrange
            UrlTrackerSettings.Value = new UrlTrackerSettings(false, false, true, true, false, false, 5000, true, 60 * 48, true);

            // act
            bool result = await _testSubject.EvaluateCandidateAsync(new ProcessedEventArgs(HttpContextMock.Context, Url.Parse("http://example.com")));

            // assert
            Assert.That(result, Is.False);
        }

        [TestCase(TestName = "EvaluateCandidateAsync returns false if the incoming url is on the regex blacklist")]
        public async Task EvaluateCandidateAsync_CandidateBlacklisted_ReturnsFalse()
        {
            // arrange
            Uri url = new Uri("http://example.com/favicon.ico");
            HttpContextMock = new HttpContextMock(url);
            UrlTrackerSettings.Value = new UrlTrackerSettings(false, false, true, false, false, false, 5000, true, 60 * 48, true);

            // act
            bool result = await _testSubject.EvaluateCandidateAsync(new ProcessedEventArgs(HttpContextMock.Context, Url.FromAbsoluteUri(url)));

            // assert
            Assert.That(result, Is.False);
        }
    }
}
