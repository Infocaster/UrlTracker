using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Umbraco.Cms.Core.Persistence.Querying;
using UrlTracker.Core.Database;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Database.Models;

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
        private readonly IInterceptCache _interceptCache;

        /// <inheritdoc />
        public DecoratorRedirectRepositoryCaching(IRedirectRepository decoratee,
                                                  IMemoryCache memoryCache,
                                                  IInterceptCache interceptCache)
        {
            _decoratee = decoratee;
            _memoryCache = memoryCache;
            _interceptCache = interceptCache;
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
            ClearCaches();
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
            return _decoratee.GetAsync(urlsAndPaths);
        }

        /// <inheritdoc/>
        public Task<RedirectEntityCollection> GetAsync(uint skip, uint take, string? query, OrderBy order, bool descending)
        {
            return _decoratee.GetAsync(skip, take, query, order, descending);
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
            });
        }

        /// <inheritdoc/>
        public void Save(IRedirect entity)
        {
            _decoratee.Save(entity);
            ClearCaches();
        }

        private void ClearCaches()
        {
            _memoryCache.Remove(Defaults.Cache.RegexRedirectKey);
            _interceptCache.Clear();
        }
    }
}
