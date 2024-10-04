using System;
using System.Threading.Tasks;
using NUnit.Framework;
using UrlTracker.Core.Models;
using UrlTracker.Resources.Testing.Mocks;
using UrlTracker.Web.Processing.Filtering;

namespace UrlTracker.Web.Tests.Processing
{
    public class ConstantsClientErrorFilterTests
    {
        private ConstantsClientErrorFilter? _testSubject;

        [SetUp]
        public void SetUp()
        {
            _testSubject = new ConstantsClientErrorFilter();
        }

        [TestCase(TestName = "EvaluateCandidateAsync returns true if the incoming request is a candidate")]
        public async Task EvaluateCandidateAsync_ValidCandidate_ReturnsTrue()
        {
            // arrange
            const string url = "http://example.com/lorem";
            var httpContextMock = new HttpContextMock(new Uri(url));

            // act
            httpContextMock.SetupUrl(Url.Parse(url));
            bool result = await _testSubject!.EvaluateCandidateAsync(httpContextMock.Context);

            // assert
            Assert.That(result, Is.True);
        }

        [TestCase(TestName = "EvaluateCandidateAsync returns false if the incoming url is on the regex blacklist")]
        public async Task EvaluateCandidateAsync_CandidateBlacklisted_ReturnsFalse()
        {
            // arrange
            const string url = "http://example.com/favicon.ico";
            var httpContextMock = new HttpContextMock(new Uri(url));

            // act
            httpContextMock.SetupUrl(Url.Parse(url));
            bool result = await _testSubject!.EvaluateCandidateAsync(httpContextMock.Context);

            // assert
            Assert.That(result, Is.False);
        }
    }
}
