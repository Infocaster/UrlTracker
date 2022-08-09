using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Resources.Testing;
using UrlTracker.Resources.Testing.Mocks;
using UrlTracker.Web.Events.Models;
using UrlTracker.Web.Processing;

namespace UrlTracker.Web.Tests.Processing
{
    public class BlacklistedUrlsClientErrorFilterTests : TestBase
    {
        private BlacklistedUrlsClientErrorFilter? _testSubject;

        public override void SetUp()
        {
            _testSubject = new BlacklistedUrlsClientErrorFilter(UrlTrackerSettings!);
        }

        [TestCase(TestName = "EvaluateCandidateAsync returns true if the incoming request is not blacklisted")]
        public async Task EvaluateCandidateAsync_ValidCandidate_ReturnsTrue()
        {
            // arrange
            UrlTrackerSettings.Value.BlockedUrlsList = new List<string>()
            {
                "item1",
                "item2",
                "ipsum"
            };
            const string url = "http://example.com/lorem";
            HttpContextMock = new HttpContextMock(new Uri(url));

            // act
            bool result = await _testSubject!.EvaluateCandidateAsync(new UrlTrackerHandled(HttpContextMock.Context, Url.Parse(url)));

            // assert
            Assert.That(result, Is.True);
        }

        [TestCase(TestName = "EvaluateCandidateAsync returns true if the blacklist is empty")]
        public async Task EvaluateCandidateAsync_ValidCandidateEmptyBlacklist_ReturnsTrue()
        {
            // arrange
            UrlTrackerSettings.Value.BlockedUrlsList = new List<string>();
            const string url = "http://example.com/blablabla";
            HttpContextMock = new HttpContextMock(new Uri(url));

            // act
            bool result = await _testSubject!.EvaluateCandidateAsync(new UrlTrackerHandled(HttpContextMock.Context, Url.Parse(url)));

            // assert
            Assert.That(result, Is.True);
        }

        [TestCase(TestName = "EvaluateCandidateAsync returns false if the incoming request is blacklisted")]
        public async Task EvaluateCandidateAsync_BlacklistedCandidate_ReturnsFalse()
        {
            // arrange
            UrlTrackerSettings.Value.BlockedUrlsList = new List<string>()
            {
                "item1",
                "item2",
                "blablabla"
            };
            const string url = "http://example.com/blablabla";
            HttpContextMock = new HttpContextMock(new Uri(url));

            // act
            bool result = await _testSubject!.EvaluateCandidateAsync(new UrlTrackerHandled(HttpContextMock.Context, Url.Parse(url)));

            // assert
            Assert.That(result, Is.False);
        }
    }
}
