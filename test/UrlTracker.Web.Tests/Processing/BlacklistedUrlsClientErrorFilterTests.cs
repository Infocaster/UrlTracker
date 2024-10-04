using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Core.Models;
using UrlTracker.Resources.Testing.Mocks;
using UrlTracker.Web.Processing.Filtering;

namespace UrlTracker.Web.Tests.Processing
{
    public class BlacklistedUrlsClientErrorFilterTests
    {
        private Mock<IOptionsMonitor<UrlTrackerSettings>> _urlTrackerSettingsMock;
        private BlacklistedUrlsClientErrorFilter? _testSubject;

        [SetUp]
        public void SetUp()
        {
            _urlTrackerSettingsMock = new Mock<IOptionsMonitor<UrlTrackerSettings>>();
            _urlTrackerSettingsMock.SetupGet(obj => obj.CurrentValue).Returns(new UrlTrackerSettings());
            _testSubject = new BlacklistedUrlsClientErrorFilter(_urlTrackerSettingsMock.Object);
        }

        [TestCase(TestName = "EvaluateCandidateAsync returns true if the incoming request is not blacklisted")]
        public async Task EvaluateCandidateAsync_ValidCandidate_ReturnsTrue()
        {
            // arrange
            _urlTrackerSettingsMock.Object.CurrentValue.BlockedUrlsList = new List<string>()
            {
                "item1",
                "item2",
                "ipsum"
            };
            const string url = "http://example.com/lorem";
            var httpContextMock = new HttpContextMock(new Uri(url));

            // act
            httpContextMock.SetupUrl(Url.Parse(url));
            bool result = await _testSubject!.EvaluateCandidateAsync(httpContextMock.Context);

            // assert
            Assert.That(result, Is.True);
        }

        [TestCase(TestName = "EvaluateCandidateAsync returns true if the blacklist is empty")]
        public async Task EvaluateCandidateAsync_ValidCandidateEmptyBlacklist_ReturnsTrue()
        {
            // arrange
            _urlTrackerSettingsMock.Object.CurrentValue.BlockedUrlsList = new List<string>();
            const string url = "http://example.com/blablabla";
            var httpContextMock = new HttpContextMock(new Uri(url));

            // act
            httpContextMock.SetupUrl(Url.Parse(url));
            bool result = await _testSubject!.EvaluateCandidateAsync(httpContextMock.Context);

            // assert
            Assert.That(result, Is.True);
        }

        [TestCase(TestName = "EvaluateCandidateAsync returns false if the incoming request is blacklisted")]
        public async Task EvaluateCandidateAsync_BlacklistedCandidate_ReturnsFalse()
        {
            // arrange
            _urlTrackerSettingsMock.Object.CurrentValue.BlockedUrlsList = new List<string>()
            {
                "item1",
                "item2",
                "blablabla"
            };
            const string url = "http://example.com/blablabla";
            var httpContextMock = new HttpContextMock(new Uri(url));

            // act
            httpContextMock.SetupUrl(Url.Parse(url));
            bool result = await _testSubject!.EvaluateCandidateAsync(httpContextMock.Context);

            // assert
            Assert.That(result, Is.False);
        }
    }
}
