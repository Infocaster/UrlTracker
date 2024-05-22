using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Persistence.Querying;
using UrlTracker.Core.Caching.Memory.Active;
using UrlTracker.Core.Caching.Memory.Notifications;
using UrlTracker.Core.Caching.Memory.Options;
using UrlTracker.Core.Database;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Core.Caching.Memory.Database
{
    /// <summary>
    /// A decorator on <see cref="IRedirectRepository" /> that caches all regex redirects in-memory
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DecoratorRedirectRepositoryCaching
        : IRedirectRepository
    {
        private readonly IRedirectRepository _decoratee;
        private readonly IMemoryCache _memoryCache;
        private readonly DistributedCache _distributedCache;
        private readonly IActiveCacheAccessor _cacheAccessor;
        private readonly IOptions<UrlTrackerMemoryCacheOptions> _options;

        /// <inheritdoc />
        public DecoratorRedirectRepositoryCaching(IRedirectRepository decoratee,
                                                  IMemoryCache memoryCache,
                                                  DistributedCache distributedCache,
                                                  IActiveCacheAccessor cacheAccessor,
                                                  IOptions<UrlTrackerMemoryCacheOptions> options)
        {
            _decoratee = decoratee;
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
            _cacheAccessor = cacheAccessor;
            _options = options;
        }

        /// <inheritdoc/>
        public int Count(IQuery<IRedirect> query)
        {
            return _decoratee.Count(query);
        }

        /// <inheritdoc/>
        public void Delete(IRedirect entity)
        {
            _decoratee.Delete(entity);
            DeleteFromCache(entity.Id);
        }

        public void DeleteBulk(int[] ids)
        {
            _decoratee.DeleteBulk(ids);
        }

        /// <inheritdoc/>
        public bool Exists(int id)
        {
            return _decoratee.Exists(id);
        }

        /// <inheritdoc/>
        public IRedirect? Get(int id)
        {
            return _decoratee.Get(id);
        }

        /// <inheritdoc/>
        public IEnumerable<IRedirect> Get(IQuery<IRedirect> query)
        {
            return _decoratee.Get(query);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyCollection<IRedirect>> GetAsync(IEnumerable<string> urlsAndPaths)
        {
            return _options.Value.EnableActiveCache
                ? Task.FromResult(_cacheAccessor.GetRedirect(urlsAndPaths))
                : _decoratee.GetAsync(urlsAndPaths);
        }

        /// <inheritdoc/>
        public Task<RedirectEntityCollection> GetAsync(uint skip, uint take, string? query, RedirectFilters filters, bool descending)
        {
            return _decoratee.GetAsync(skip, take, query, filters, descending);
        }

        /// <inheritdoc/>
        public IEnumerable<IRedirect> GetMany(params int[]? ids)
        {
            return _decoratee.GetMany(ids);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyCollection<IRedirect>> GetWithRegexAsync()
        {
            return _memoryCache.GetOrCreateAsync(Defaults.Cache.RegexRedirectKey, e =>
            {
                return _decoratee.GetWithRegexAsync();
            })!;
        }

        /// <inheritdoc/>
        public void Save(IRedirect entity)
        {
            _decoratee.Save(entity);
            UpdateFromCache(entity.Id);
        }

        private void UpdateFromCache(int id)
        {
            _distributedCache.Refresh(RedirectsCacheRefresher.UniqueKey, id);
        }

        private void DeleteFromCache(int id)
        {
            _distributedCache.Remove(RedirectsCacheRefresher.UniqueKey, id);
        }
    }
}
