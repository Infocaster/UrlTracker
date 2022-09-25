using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Core.Cache;
using Umbraco.Extensions;
using UrlTracker.Core.Domain;
using UrlTracker.Core.Domain.Models;

namespace UrlTracker.Core.Caching.Memory.Domain
{
    /// <summary>
    /// A decorator on <see cref="IDomainProvider"/> that caches all Umbraco domains in-memory
    /// </summary>
    public class DecoratorDomainProviderCaching
        : IDomainProvider
    {
        private readonly IDomainProvider _decoratee;
        private readonly IAppPolicyCache _runtimeCache;

        /// <inheritdoc />
        public DecoratorDomainProviderCaching(IDomainProvider decoratee, IAppPolicyCache runtimeCache)
        {
            _decoratee = decoratee;
            _runtimeCache = runtimeCache;
        }

        /// <inheritdoc/>
        public DomainCollection GetDomains()
        {
            return _runtimeCache.GetCacheItem(Defaults.Cache.DomainKey, () => _decoratee.GetDomains())!;
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public DomainCollection GetDomains(int nodeId)
        {
            // Right now, this method is only used for backoffice api controllers, which don't have any performance requirements
            //    Therefore, I chose not to implement any caching here
            return _decoratee.GetDomains(nodeId);
        }
    }
}
