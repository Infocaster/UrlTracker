using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using UrlTracker.Core.Caching;
using UrlTracker.Core.Database.Models;

namespace UrlTracker.Core.Database
{
    [Obsolete("Introduced because any entry might be removed through this service. Do not use for new features.")]
    [ExcludeFromCodeCoverage]
    public class DecoratorLegacyRepositoryCaching : ILegacyRepository
    {
        private readonly ILegacyRepository _decoratee;
        private readonly IMemoryCache _memoryCache;
        private readonly IInterceptCache _interceptCache;

        public DecoratorLegacyRepositoryCaching(ILegacyRepository decoratee, IMemoryCache memoryCache, IInterceptCache interceptCache)
        {
            _decoratee = decoratee;
            _memoryCache = memoryCache;
            _interceptCache = interceptCache;
        }

        public async Task DeleteAsync(UrlTrackerEntry entry)
        {
            await _decoratee.DeleteAsync(entry);
            _memoryCache.Remove(Defaults.Cache.RegexRedirectKey);
            _interceptCache.Clear();
        }

        public async Task DeleteAsync(string? culture, string sourceUrl, int? targetRootNodeId, bool is404)
        {
            await _decoratee.DeleteAsync(culture, sourceUrl, targetRootNodeId, is404);
            _memoryCache.Remove(Defaults.Cache.RegexRedirectKey);
            _interceptCache.Clear();
        }

        public Task<UrlTrackerEntry?> GetAsync(int id)
        {
            return _decoratee.GetAsync(id);
        }

        public Task<bool> IsIgnoredAsync(string url)
        {
            return _decoratee.IsIgnoredAsync(url);
        }
    }
}
