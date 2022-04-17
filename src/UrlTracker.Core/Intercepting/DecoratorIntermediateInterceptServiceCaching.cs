using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using UrlTracker.Core.Caching;
using UrlTracker.Core.Configuration;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Intercepting.Models;

namespace UrlTracker.Core.Intercepting
{
    public class DecoratorIntermediateInterceptServiceCaching
        : IIntermediateInterceptService
    {
        private readonly IIntermediateInterceptService _decoratee;
        private readonly IInterceptCache _interceptCache;
        private readonly IConfiguration<UrlTrackerSettings> _options;
        public DecoratorIntermediateInterceptServiceCaching(IIntermediateInterceptService decoratee,
                                                            IInterceptCache interceptCache,
                                                            IConfiguration<UrlTrackerSettings> options)
        {
            _decoratee = decoratee;
            _interceptCache = interceptCache;
            _options = options;
        }

        public Task<ICachableIntercept> GetAsync(Url url, IInterceptContext context = null)
        {
            var slidingCacheMinutes = _options.Value.InterceptSlidingCacheMinutes;
            var cacheOptions = new MemoryCacheEntryOptions();
            if (slidingCacheMinutes != null) cacheOptions.SetSlidingExpiration(TimeSpan.FromMinutes(slidingCacheMinutes.Value));
            return _interceptCache.GetOrCreateAsync(url, () => _decoratee.GetAsync(url, context), cacheOptions);
        }
    }
}
