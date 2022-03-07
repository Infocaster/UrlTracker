using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using UrlTracker.Core.Database.Models;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Intercepting;
using UrlTracker.Resources.Testing;
using UrlTracker.Resources.Testing.Logging;

namespace UrlTracker.Core.Tests.Intercepting
{
    public class StaticUrlRedirectMatcherTests : TestBase
    {
        private StaticUrlRedirectInterceptor _redirectMatcher;

        public override void SetUp()
        {
            _redirectMatcher = new StaticUrlRedirectInterceptor(RedirectRepository, new StaticUrlProviderCollection(() => new List<IStaticUrlProvider> { new StaticUrlProvider() }), new ConsoleLogger<StaticUrlRedirectInterceptor>());
        }

        public static IEnumerable<TestCaseData> TestCases()
        {
            var result1 = new UrlTrackerShallowRedirect
            {
                SourceUrl = "http://example.com/"
            };
            var result2 = new UrlTrackerShallowRedirect
            {
                SourceUrl = "http://example.com/lorem"
            };

            yield return new TestCaseData(
                new UrlTrackerShallowRedirect[0],
                null).SetName("InterceptAsync returns null if no match is found");

            yield return new TestCaseData(
                new UrlTrackerShallowRedirect[] { result1, result2 },
                result1).SetName("InterceptAsync returns first result if any are found");
        }

        [TestCaseSource(nameof(TestCases))]
        public async Task GetMatchAsync_NormalFlow_ReturnsMatch(UrlTrackerShallowRedirect[] output, UrlTrackerShallowRedirect expected)
        {
            // arrange
            RedirectRepositoryMock.Setup(obj => obj.GetShallowAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<int?>(), It.IsAny<string>())).ReturnsAsync(output);

            // act
            var result = await _redirectMatcher.InterceptAsync(Url.Parse("http://example.com"), DefaultInterceptContext);

            // assert
            Assert.That(result?.Info, Is.EqualTo(expected));
        }
    }
}
