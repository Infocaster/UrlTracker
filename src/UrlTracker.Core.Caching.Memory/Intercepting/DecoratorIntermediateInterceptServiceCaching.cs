using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using UrlTracker.Core.Caching.Memory.Options;
using UrlTracker.Core.Intercepting;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Caching.Memory.Intercepting
{
    /// <summary>
    /// A decorator on <see cref="IIntermediateInterceptService" /> that caches intercepts by url in-memory
    /// </summary>
    public class DecoratorIntermediateInterceptServiceCaching
        : IIntermediateInterceptService
    {
        private readonly IIntermediateInterceptService _decoratee;
        private readonly IInterceptCache _interceptCache;
        private readonly IOptions<UrlTrackerMemoryCacheOptions> _options;

        /// <inheritdoc />
        public DecoratorIntermediateInterceptServiceCaching(IIntermediateInterceptService decoratee,
                                                            IInterceptCache interceptCache,
                                                            IOptions<UrlTrackerMemoryCacheOptions> options)
        {
            _decoratee = decoratee;
            _interceptCache = interceptCache;
            _options = options;
        }

        /// <inheritdoc/>
        public Task<ICachableIntercept> GetAsync(Url url, IInterceptContext? context = null)
        {
            var slidingCacheMinutes = _options.Value.InterceptSlidingCacheMinutes;
            var cacheOptions = new MemoryCacheEntryOptions();
            if (slidingCacheMinutes is not null) cacheOptions.SetSlidingExpiration(TimeSpan.FromMinutes(slidingCacheMinutes.Value));

            return _interceptCache.GetOrCreateAsync(url, () => _decoratee.GetAsync(url, context), cacheOptions);
        }
    }
}
