using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Services;
using UrlTracker.Core.Caching.Memory.Options;

namespace UrlTracker.Core.Caching.Memory.Active
{
    internal class ActiveCacheBootloader
        : IAsyncComponent
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

        public Task InitializeAsync(bool isRestarting, CancellationToken cancellationToken)
        {
            if (_runtimeState.Level < RuntimeLevel.Run || !_options.Value.EnableActiveCache) return Task.CompletedTask;

            _redirectCacheWriter.RefreshRedirects();
            _clientErrorCacheWriter.RefreshNoLongerExists();
            return Task.CompletedTask;
        }

        public Task TerminateAsync(bool isRestarting, CancellationToken cancellationToken)
        {
            // Terminate is not required, but part of the interface
            return Task.CompletedTask;
        }
    }
}
