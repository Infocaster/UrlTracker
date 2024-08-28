using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using Umbraco.Cms.Infrastructure.Scoping;
using UrlTracker.Core.Database;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Core.Caching.Memory.Active
{
    internal interface IActiveRedirectCacheWriter
    {
        void RefreshRedirects();
        void RefreshRedirect(int id);
        void RemoveRedirect(int id);
    }

    internal class ActiveRedirectCacheWriter : IActiveRedirectCacheWriter
    {
        private readonly IRedirectRepository _redirectRepository;
        private readonly IScopeProvider _scopeProvider;
        private readonly IActiveCacheAccessor _cacheAccessor;
        private readonly IMemoryCache _memoryCache;
        private readonly IInterceptCache _interceptCache;

        public ActiveRedirectCacheWriter(
            IRedirectRepository redirectRepository,
            IScopeProvider scopeProvider,
            IActiveCacheAccessor cacheAccessor,
            IMemoryCache memoryCache,
            IInterceptCache interceptCache)
        {
            _redirectRepository = redirectRepository;
            _scopeProvider = scopeProvider;
            _cacheAccessor = cacheAccessor;
            _memoryCache = memoryCache;
            _interceptCache = interceptCache;
        }

        public void RefreshRedirect(int id)
        {
            // delete first
            RemoveRedirect(id);

            // fetch from the database
            using var scope = _scopeProvider.CreateScope();
            var redirect = _redirectRepository.Get(id);

            if (redirect is not null && redirect.Source.Strategy == Core.Defaults.DatabaseSchema.RedirectSourceStrategies.Url)
            {
                // insert redirect in expected location
                var cache = _cacheAccessor.GetRedirectCache();
                if (!cache.TryGetValue(redirect.Source.Value, out var list))
                {
                    list = new List<IRedirect>();
                    cache[redirect.Source.Value] = list;
                }

                list.Add(redirect);
            }

            ClearLazyCaches();

            scope.Complete();
        }

        public void RefreshRedirects()
        {
            using var scope = _scopeProvider.CreateScope();

            var query = scope.SqlContext.Query<IRedirect>()
                .Where(e => e.SourceStrategy == Core.Defaults.DatabaseSchema.RedirectSourceStrategies.Url);

            var urlRedirects = _redirectRepository.Get(query);

            var result = urlRedirects
                .Where(e => e.Source.Strategy == Core.Defaults.DatabaseSchema.RedirectSourceStrategies.Url)
                .GroupBy(e => e.Source.Value.ToLower())
                .ToDictionary(g => g.Key, g => g.ToList());

            _cacheAccessor.Set(result);

            ClearLazyCaches();

            scope.Complete();
        }

        public void RemoveRedirect(int id)
        {
            var cache = _cacheAccessor.GetRedirectCache();
            foreach (var key in cache.Keys.ToList())
            {
                var value = cache[key];
                value.RemoveAll(r => r.Id == id);
                if (value.Count == 0)
                {
                    cache.Remove(key);
                }
            }

            ClearLazyCaches();
        }

        private void ClearLazyCaches()
        {
            _memoryCache.Remove(Defaults.Cache.RegexRedirectKey);
            _interceptCache.Clear();
        }
    }
}
