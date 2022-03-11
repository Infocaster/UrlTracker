using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Core.Cache;
using Umbraco.Extensions;
using UrlTracker.Core.Domain.Models;

namespace UrlTracker.Core.Domain
{
    public class DecoratorDomainProviderCaching
        : IDomainProvider
    {
        private readonly IDomainProvider _decoratee;
        private readonly IAppPolicyCache _runtimeCache;

        public DecoratorDomainProviderCaching(IDomainProvider decoratee, IAppPolicyCache runtimeCache)
        {
            _decoratee = decoratee;
            _runtimeCache = runtimeCache;
        }

        public DomainCollection GetDomains()
        {
            return _runtimeCache.GetCacheItem(Defaults.Cache.DomainKey, () => _decoratee.GetDomains());
        }

        [ExcludeFromCodeCoverage]
        public DomainCollection GetDomains(int nodeId)
        {
            // Right now, this method is only used for backoffice api controllers, which don't have any performance requirements
            //    Therefore, I chose not to implement any caching here
            return _decoratee.GetDomains(nodeId);
        }
    }
}
