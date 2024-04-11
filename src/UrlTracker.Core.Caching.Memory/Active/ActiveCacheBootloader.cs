using Microsoft.Extensions.Options;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Services;
using UrlTracker.Core.Caching.Memory.Options;

namespace UrlTracker.Core.Caching.Memory.Active
{
    internal class ActiveCacheBootloader
        : IComponent
    {
        private readonly IActiveRedirectCacheWriter _redirectCacheWriter;
        private readonly IActiveClientErrorCacheWriter _clientErrorCacheWriter;
        private readonly IOptions<UrlTrackerMemoryCacheOptions> _options;
        private readonly IRuntimeState _runtimeState;

        public ActiveCacheBootloader(
            IActiveRedirectCacheWriter redirectCacheWriter,
            IActiveClientErrorCacheWriter clientErrorCacheWriter,
            IOptions<UrlTrackerMemoryCacheOptions> options,
            IRuntimeState runtimeState)
        {
            _redirectCacheWriter = redirectCacheWriter;
            _clientErrorCacheWriter = clientErrorCacheWriter;
            _options = options;
            _runtimeState = runtimeState;
        }

        public void Initialize()
        {
            if (_runtimeState.Level < RuntimeLevel.Run || !_options.Value.EnableActiveCache) return;

            _redirectCacheWriter.RefreshRedirects();
            _clientErrorCacheWriter.RefreshNoLongerExists();
        }

        public void Terminate()
        {
            // Terminate is not required, but part of the interface
        }
    }
}
