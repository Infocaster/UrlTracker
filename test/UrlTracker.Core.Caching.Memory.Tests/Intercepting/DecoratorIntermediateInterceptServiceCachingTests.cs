using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using UrlTracker.Core.Caching.Memory.Intercepting;
using UrlTracker.Core.Caching.Memory.Options;
using UrlTracker.Core.Intercepting;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Caching.Memory.Tests.Intercepting
{
    public class DecoratorIntermediateInterceptServiceCachingTests
    {
        private Mock<IIntermediateInterceptService> _intermediateInterceptServiceMock;
        private Mock<IInterceptCache> _interceptCacheMock;
        private IOptions<UrlTrackerMemoryCacheOptions> _urlTrackerMemoryCacheOptions;
        private DecoratorIntermediateInterceptServiceCaching _testSubject = null!;

        [SetUp]
        public void SetUp()
        {
            _intermediateInterceptServiceMock = new Mock<IIntermediateInterceptService>();
            _interceptCacheMock = new Mock<IInterceptCache>();
            _urlTrackerMemoryCacheOptions = Microsoft.Extensions.Options.Options.Create(new UrlTrackerMemoryCacheOptions());
            _testSubject = new DecoratorIntermediateInterceptServiceCaching(_intermediateInterceptServiceMock.Object, _interceptCacheMock.Object, _urlTrackerMemoryCacheOptions);
        }

        public static IEnumerable<TestCaseData> TestCases()
        {
            yield return new TestCaseData(TimeSpan.FromMinutes(2)).SetName("GetAsync sets sliding cache if not null");
            yield return new TestCaseData(null).SetName("GetAsync does not set sliding cache if null");
        }

        [TestCaseSource(nameof(TestCases))]
        public async Task GetAsync_NormalFlow_SetsSlidingCache(TimeSpan? slidingCache)
        {
            // arrange
            _urlTrackerMemoryCacheOptions.Value.InterceptSlidingCacheMinutes = (int?)slidingCache?.TotalMinutes;
            _interceptCacheMock.Setup(obj => obj.GetOrCreateAsync(It.IsAny<Url>(),
                                                                  It.IsAny<Func<Task<ICachableIntercept>>>(),
                                                                  It.Is<MemoryCacheEntryOptions?>(x => x!.SlidingExpiration == slidingCache)))
                               .Returns((Url url, Func<Task<ICachableIntercept>> factory, MemoryCacheEntryOptions cache) => factory())
                               .Verifiable();
            _intermediateInterceptServiceMock!.Setup(obj => obj.GetAsync(It.IsAny<Url>(), It.IsAny<IInterceptContext?>()))
                                             .ReturnsAsync(CachableInterceptBase.NullIntercept);

            // act
            await _testSubject!.GetAsync(Url.Parse("http://example.com"));

            // assert
            _interceptCacheMock.Verify();
        }
    }
}
