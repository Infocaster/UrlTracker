using System.Linq;
using Umbraco.Cms.Infrastructure.Scoping;
using UrlTracker.Core.Database;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Core.Caching.Memory.Active
{
    public interface IActiveClientErrorCacheWriter
    {
        void RefreshNoLongerExists();
    }

    internal class ActiveClientErrorCacheWriter : IActiveClientErrorCacheWriter
    {
        private readonly IActiveCacheAccessor _cacheAccessor;
        private readonly IScopeProvider _scopeProvider;
        private readonly IClientErrorRepository _clientErrorRepository;

        public ActiveClientErrorCacheWriter(IClientErrorRepository clientErrorRepository, IScopeProvider scopeProvider, IActiveCacheAccessor cacheAccessor)
        {
            _cacheAccessor = cacheAccessor;
            _scopeProvider = scopeProvider;
            _clientErrorRepository = clientErrorRepository;
        }

        public void RefreshNoLongerExists()
        {
            using var scope = _scopeProvider.CreateScope();

            var query = scope.SqlContext.Query<IClientError>()
                .Where(e => e.Strategy == Core.Defaults.DatabaseSchema.ClientErrorStrategies.NoLongerExists);

            var clientErrors = _clientErrorRepository.Get(query);
            var result = clientErrors
                .GroupBy(e => e.Url.ToLower())
                .ToDictionary(g => g.Key, g => g.ToList());

            _cacheAccessor.Set(result);
        }
    }
}
