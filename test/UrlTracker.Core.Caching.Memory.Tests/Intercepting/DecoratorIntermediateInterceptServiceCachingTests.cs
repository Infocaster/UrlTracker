using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using UrlTracker.Core.Caching.Memory.Intercepting;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Resources.Testing;

namespace UrlTracker.Core.Caching.Memory.Tests.Intercepting
{
    public class DecoratorIntermediateInterceptServiceCachingTests : TestBase
    {
        private DecoratorIntermediateInterceptServiceCaching _testSubject = null!;

        public override void SetUp()
        {
            _testSubject = new DecoratorIntermediateInterceptServiceCaching(IntermediateInterceptService, InterceptCache, UrlTrackerMemoryCacheOptions);
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
            UrlTrackerMemoryCacheOptions.Value.InterceptSlidingCacheMinutes = (int?)slidingCache?.TotalMinutes;
            InterceptCacheMock.Setup(obj => obj.GetOrCreateAsync(It.IsAny<Url>(),
                                                                  It.IsAny<Func<Task<ICachableIntercept>>>(),
                                                                  It.Is<MemoryCacheEntryOptions?>(x => x!.SlidingExpiration == slidingCache)))
                               .Returns((Url url, Func<Task<ICachableIntercept>> factory, MemoryCacheEntryOptions cache) => factory())
                               .Verifiable();
            IntermediateInterceptServiceMock!.Setup(obj => obj.GetAsync(It.IsAny<Url>(), It.IsAny<IInterceptContext?>()))
                                             .ReturnsAsync(CachableInterceptBase.NullIntercept);

            // act
            await _testSubject!.GetAsync(Url.Parse("http://example.com"));

            // assert
            InterceptCacheMock.Verify();
        }
    }
}
