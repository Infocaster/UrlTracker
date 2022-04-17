using System;
using System.Threading.Tasks;
using UrlTracker.Core.Caching;
using UrlTracker.Core.Database.Models;

namespace UrlTracker.Core.Database
{
    [Obsolete("Introduced because any entry might be removed through this service. Do not use for new features.")]
    public class DecoratorLegacyRepositoryCaching : ILegacyRepository
    {
        private readonly ILegacyRepository _decoratee;
        private readonly IRegexRedirectCache _regexRedirectCache;
        private readonly IInterceptCache _interceptCache;
        public DecoratorLegacyRepositoryCaching(ILegacyRepository decoratee, IRegexRedirectCache regexRedirectCache, IInterceptCache interceptCache)
        {
            _decoratee = decoratee;
            _regexRedirectCache = regexRedirectCache;
            _interceptCache = interceptCache;
        }
        public async Task DeleteAsync(UrlTrackerEntry entry)
        {
            await _decoratee.DeleteAsync(entry);
            _regexRedirectCache.Clear();
            _interceptCache.Clear();
        }
        public async Task DeleteAsync(string culture, string sourceUrl, int? targetRootNodeId, bool is404)
        {
            await _decoratee.DeleteAsync(culture, sourceUrl, targetRootNodeId, is404);
            _regexRedirectCache.Clear();
            _interceptCache.Clear();
        }
        public Task<UrlTrackerEntry> GetAsync(int id)
        {
            return _decoratee.GetAsync(id);
        }
    }
}
