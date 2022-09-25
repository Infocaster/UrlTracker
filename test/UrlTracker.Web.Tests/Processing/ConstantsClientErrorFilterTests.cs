using System;
using System.Threading.Tasks;
using NUnit.Framework;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Resources.Testing;
using UrlTracker.Resources.Testing.Mocks;
using UrlTracker.Web.Processing;

namespace UrlTracker.Web.Tests.Processing
{
    public class ConstantsClientErrorFilterTests : TestBase
    {
        private ConstantsClientErrorFilter? _testSubject;

        public override void SetUp()
        {
            _testSubject = new ConstantsClientErrorFilter();
        }

        [TestCase(TestName = "EvaluateCandidateAsync returns true if the incoming request is a candidate")]
        public async Task EvaluateCandidateAsync_ValidCandidate_ReturnsTrue()
        {
            // arrange
            const string url = "http://example.com/lorem";
            HttpContextMock = new HttpContextMock(new Uri(url));

            // act
            HttpContextMock.SetupUrl(Url.Parse(url));
            bool result = await _testSubject!.EvaluateCandidateAsync(HttpContextMock.Context);

            // assert
            Assert.That(result, Is.True);
        }

        [TestCase(TestName = "EvaluateCandidateAsync returns false if the incoming url is on the regex blacklist")]
        public async Task EvaluateCandidateAsync_CandidateBlacklisted_ReturnsFalse()
        {
            // arrange
            const string url = "http://example.com/favicon.ico";
            HttpContextMock = new HttpContextMock(new Uri(url));

            // act
            HttpContextMock.SetupUrl(Url.Parse(url));
            bool result = await _testSubject!.EvaluateCandidateAsync(HttpContextMock.Context);

            // assert
            Assert.That(result, Is.False);
        }
    }
}
