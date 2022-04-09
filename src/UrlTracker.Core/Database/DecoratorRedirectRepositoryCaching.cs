using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using UrlTracker.Core.Database.Models;

namespace UrlTracker.Core.Database
{
    public class DecoratorRedirectRepositoryCaching
        : IRedirectRepository
    {
        private readonly IRedirectRepository _decoratee;
        private readonly IMemoryCache _memoryCache;

        public DecoratorRedirectRepositoryCaching(IRedirectRepository decoratee,
                                                  IMemoryCache memoryCache)
        {
            _decoratee = decoratee;
            _memoryCache = memoryCache;
        }

        public async Task<UrlTrackerRedirect> AddAsync(UrlTrackerRedirect redirect)
        {
            var result = await _decoratee.AddAsync(redirect);
            _memoryCache.Remove(Defaults.Cache.RegexRedirectKey);
            return result;
        }

        public Task<UrlTrackerRedirectCollection> GetAsync(uint skip, uint take, string? query, OrderBy order, bool descending)
        {
            return _decoratee.GetAsync(skip, take, query, order, descending);
        }

        public Task<UrlTrackerRedirectCollection> GetAsync()
        {
            return _decoratee.GetAsync();
        }

        public Task<IReadOnlyCollection<UrlTrackerShallowRedirect>> GetShallowAsync(IEnumerable<string> urlsAndPaths, int? rootNodeId = null, string? culture = null)
        {
            return _decoratee.GetShallowAsync(urlsAndPaths, rootNodeId, culture);
        }

        public Task<IReadOnlyCollection<UrlTrackerShallowRedirect>> GetShallowWithRegexAsync()
        {
            return _memoryCache.GetOrCreateAsync<IReadOnlyCollection<UrlTrackerShallowRedirect>>(Defaults.Cache.RegexRedirectKey, e =>
            {
                return _decoratee.GetShallowWithRegexAsync();
            });
        }

        public async Task<UrlTrackerRedirect> UpdateAsync(UrlTrackerRedirect redirect)
        {
            var result = await _decoratee.UpdateAsync(redirect);
            _memoryCache.Remove(Defaults.Cache.RegexRedirectKey);
            return result;
        }
    }
}
