using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Intercepting;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Resources.Testing;
using UrlTracker.Resources.Testing.Logging;

namespace UrlTracker.Core.Tests.Intercepting
{
    public class NoLongerExistsInterceptorTests : TestBase
    {
        private NoLongerExistsInterceptor? _testSubject;

        public override void SetUp()
        {
            _testSubject = new NoLongerExistsInterceptor(ClientErrorRepository, new StaticUrlProviderCollection(() => new List<IStaticUrlProvider> { new StaticUrlProvider() }), new VoidLogger<NoLongerExistsInterceptor>());
        }

        public static IEnumerable<TestCaseData> TestCases()
        {
            var result1 = new ClientErrorEntity("https://example.com", false, Defaults.DatabaseSchema.ClientErrorStrategies.NoLongerExists, default, default, default);
            var result2 = new ClientErrorEntity("https://example.com", false, Defaults.DatabaseSchema.ClientErrorStrategies.NotFound, default, default, default);

            yield return new TestCaseData(
                new IClientError[] { result1, result2 }, result1
                ).SetName("InterceptAsync returns the first 410 result if any are found");
            yield return new TestCaseData(
                new IClientError[] { result2 }, null
                ).SetName("InterceptAsync returns null if no 410 results are found");
        }

        [TestCaseSource(nameof(TestCases))]
        public async Task InterceptAsync_NormalFlow_ReturnsResult(IClientError[] output, IClientError expected)
        {
            // arrange
            ClientErrorRepositoryMock!.Setup(obj => obj.GetAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<int?>(), It.IsAny<string?>()))
                                     .ReturnsAsync(output);

            // act
            var result = await _testSubject!.InterceptAsync(Url.Parse("http://example.com"), new DefaultInterceptContext());

            // assert
            Assert.That(result?.Info, Is.EqualTo(expected));
        }
    }
}
