using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using UrlTracker.Core.Database.Models;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Intercepting;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Resources.Testing;
using UrlTracker.Resources.Testing.Logging;

namespace UrlTracker.Core.Tests.Intercepting
{
    public class NoLongerExistsInterceptorTests : TestBase
    {
        private NoLongerExistsInterceptor _testSubject;

        public override void SetUp()
        {
            _testSubject = new NoLongerExistsInterceptor(ClientErrorRepository, new StaticUrlProviderCollection(new List<IStaticUrlProvider> { new StaticUrlProvider() }), new ConsoleLogger());
        }

        public static IEnumerable<TestCaseData> TestCases()
        {
            var result1 = new UrlTrackerShallowClientError
            {
                TargetStatusCode = HttpStatusCode.Gone
            };
            var result2 = new UrlTrackerShallowClientError
            {
                TargetStatusCode = HttpStatusCode.NotFound
            };

            yield return new TestCaseData(
                new UrlTrackerShallowClientError[] { result1, result2 }, result1
                ).SetName("InterceptAsync returns the first 410 result if any are found");
            yield return new TestCaseData(
                new UrlTrackerShallowClientError[] { result2 }, null
                ).SetName("InterceptAsync returns null if no 410 results are found");
        }

        [TestCaseSource(nameof(TestCases))]
        public async Task InterceptAsync_NormalFlow_ReturnsResult(UrlTrackerShallowClientError[] output, UrlTrackerShallowClientError expected)
        {
            // arrange
            ClientErrorRepositoryMock.Setup(obj => obj.GetShallowAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<int?>(), It.IsAny<string>()))
                                     .ReturnsAsync(output);

            // act
            var result = await _testSubject.InterceptAsync(Url.Parse("http://example.com"), new DefaultInterceptContext());

            // assert
            Assert.That(result?.Info, Is.EqualTo(expected));
        }
    }
}
