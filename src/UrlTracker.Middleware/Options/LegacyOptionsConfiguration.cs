using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;
using UrlTracker.Modules.Options;

namespace UrlTracker.Middleware.Options
{
    [ExcludeFromCodeCoverage]
    internal class LegacyOptionsConfiguration : IConfigureOptions<UrlTrackerLegacyOptions>
    {
        private readonly IOptionsMonitor<UrlTrackerPipelineOptions> _pipelineOptions;

        public LegacyOptionsConfiguration(IOptionsMonitor<UrlTrackerPipelineOptions> pipelineOptions)
        {
            _pipelineOptions = pipelineOptions;
        }

        public void Configure(UrlTrackerLegacyOptions options)
        {
            var pipelineOptionsValue = _pipelineOptions.CurrentValue;
            options.IsNotFoundTrackingDisabled = !pipelineOptionsValue.EnableClientErrorTracking;
        }
    }
}
