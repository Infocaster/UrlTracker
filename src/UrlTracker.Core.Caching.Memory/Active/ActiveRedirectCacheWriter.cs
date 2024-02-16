using System.Linq;
using Umbraco.Cms.Infrastructure.Scoping;
using UrlTracker.Core.Database;
using UrlTracker.Core.Database.Models.Entities;

namespace UrlTracker.Core.Caching.Memory.Active
{
    internal interface IActiveRedirectCacheWriter
    {
        void RefreshRedirects();
    }

    internal class ActiveRedirectCacheWriter : IActiveRedirectCacheWriter
    {
        private readonly IRedirectRepository _redirectRepository;
        private readonly IScopeProvider _scopeProvider;
        private readonly IActiveCacheAccessor _cacheAccessor;

        public ActiveRedirectCacheWriter(IRedirectRepository redirectRepository, IScopeProvider scopeProvider, IActiveCacheAccessor cacheAccessor)
        {
            _redirectRepository = redirectRepository;
            _scopeProvider = scopeProvider;
            _cacheAccessor = cacheAccessor;
        }

        public void RefreshRedirects()
        {
            using var scope = _scopeProvider.CreateScope();

            var query = scope.SqlContext.Query<IRedirect>()
                .Where(e => e.SourceUrl != null && e.SourceUrl != string.Empty);

            var urlRedirects = _redirectRepository.Get(query);

            var result = urlRedirects
                .Where(e => e.SourceUrl is not null)
                .GroupBy(e => e.SourceUrl!.ToLower())
                .ToDictionary(g => g.Key, g => g.ToList());

            _cacheAccessor.Set(result);
            scope.Complete();
        }
    }
}
