using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Umbraco.Core.Persistence.Querying;
using UrlTracker.Core.Caching;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Database.Models;
using UrlTracker.Core.Database.Models.Entities;

namespace UrlTracker.Core.Database
{
    [ExcludeFromCodeCoverage]
    public class DecoratorRedirectRepositoryCaching
        : IRedirectRepository
    {
        private readonly IRedirectRepository _decoratee;
        private readonly IRegexRedirectCache _regexRedirectCache;
        private readonly IInterceptCache _interceptCache;

        public DecoratorRedirectRepositoryCaching(IRedirectRepository decoratee,
                                                  IRegexRedirectCache regexRedirectCache,
                                                  IInterceptCache interceptCache)
        {
            _decoratee = decoratee;
            _regexRedirectCache = regexRedirectCache;
            _interceptCache = interceptCache;
        }

        public int Count(IQuery<IRedirect> query)
        {
            return _decoratee.Count(query);
        }

        public void Delete(IRedirect entity)
        {
            _decoratee.Delete(entity);
            ClearCaches();
        }

        public bool Exists(int id)
        {
            return _decoratee.Exists(id);
        }

        public IRedirect Get(int id)
        {
            return _decoratee.Get(id);
        }

        public IEnumerable<IRedirect> Get(IQuery<IRedirect> query)
        {
            return _decoratee.Get(query);
        }

        public Task<IReadOnlyCollection<IRedirect>> GetAsync(IEnumerable<string> urlsAndPaths, int? rootNodeId = null, string culture = null)
        {
            return _decoratee.GetAsync(urlsAndPaths, rootNodeId, culture);
        }

        public Task<RedirectEntityCollection> GetAsync(uint skip, uint take, string query, OrderBy order, bool descending)
        {
            return _decoratee.GetAsync(skip, take, query, order, descending);
        }

        public IEnumerable<IRedirect> GetMany(params int[] ids)
        {
            return _decoratee.GetMany(ids);
        }

        public Task<IReadOnlyCollection<IRedirect>> GetWithRegexAsync()
        {
            return _regexRedirectCache.GetOrCreateAsync(Defaults.Cache.RegexRedirectKey, () =>
            {
                return _decoratee.GetWithRegexAsync();
            });
        }

        public void Save(IRedirect entity)
        {
            _decoratee.Save(entity);
            ClearCaches();
        }

        private void ClearCaches()
        {
            _regexRedirectCache.Clear();
            _interceptCache.Clear();
        }
    }
}
