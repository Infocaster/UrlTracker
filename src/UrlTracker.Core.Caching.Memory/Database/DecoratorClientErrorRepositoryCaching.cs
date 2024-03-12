using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Persistence.Querying;
using UrlTracker.Core.Caching.Memory.Active;
using UrlTracker.Core.Caching.Memory.Options;
using UrlTracker.Core.Database;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Database.Models;

namespace UrlTracker.Core.Caching.Memory.Database
{
    internal class DecoratorClientErrorRepositoryCaching
        : IClientErrorRepository
    {
        private readonly IClientErrorRepository _decoratee;
        private readonly IActiveCacheAccessor _cacheAccessor;
        private readonly IOptions<UrlTrackerMemoryCacheOptions> _options;

        public DecoratorClientErrorRepositoryCaching(
            IClientErrorRepository decoratee,
            IActiveCacheAccessor cacheAccessor,
            IOptions<UrlTrackerMemoryCacheOptions> options)
        {
            _decoratee = decoratee;
            _cacheAccessor = cacheAccessor;
            _options = options;
        }

        public int Count(IQuery<IClientError> query)
        {
            return _decoratee.Count(query);
        }

        public Task<int> CountAsync(DateTime start, DateTime end)
        {
            return _decoratee.CountAsync(start, end);
        }

        public void Delete(IClientError entity)
        {
            _decoratee.Delete(entity);
        }

        public bool Exists(int id)
        {
            return _decoratee.Exists(id);
        }

        public IClientError? Get(int id)
        {
            return _decoratee.Get(id);
        }

        public IEnumerable<IClientError> Get(IQuery<IClientError> query)
        {
            return _decoratee.Get(query);
        }

        public Task<ClientErrorEntityCollection> GetAsync(uint skip, uint take, string? query, OrderBy order, bool descending)
        {
            return _decoratee.GetAsync(skip, take, query, order, descending);
        }

        public Task<IReadOnlyCollection<IClientError>> GetAsync(IEnumerable<string> urlsAndPaths, int? rootNodeId = null, string? culture = null)
        {
            return _decoratee.GetAsync(urlsAndPaths, rootNodeId, culture);
        }

        public IEnumerable<IClientError> GetMany(params int[]? ids)
        {
            return _decoratee.GetMany(ids);
        }

        public Task<IReadOnlyCollection<IClientErrorMetaData>> GetMetaDataAsync(params int[] clientErrors)
        {
            return _decoratee.GetMetaDataAsync(clientErrors);
        }

        public Task<IReadOnlyCollection<IClientError>> GetNoLongerExistsAsync(IEnumerable<string> urlsAndPaths, int? rootNodeId = null, string? culture = null)
        {
            return _options.Value.EnableActiveCache
                ? Task.FromResult(_cacheAccessor.GetNoLongerExists(urlsAndPaths))
                : _decoratee.GetNoLongerExistsAsync(urlsAndPaths, rootNodeId, culture);
        }

        public void Report(IClientError clientError, DateTime moment, IReferrer? referrer)
        {
            _decoratee.Report(clientError, moment, referrer);
        }

        public void Save(IClientError entity)
        {
            _decoratee.Save(entity);
        }
    }
}
