using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Modules.Options;

namespace UrlTracker.Core.Configuration
{
    [ExcludeFromCodeCoverage]
    internal class LegacyOptionsConfiguration : IConfigureOptions<UrlTrackerLegacyOptions>
    {
        private readonly IOptionsMonitor<UrlTrackerSettings> _urlTrackerOptions;

        public LegacyOptionsConfiguration(IOptionsMonitor<UrlTrackerSettings> urlTrackerOptions)
        {
            _urlTrackerOptions = urlTrackerOptions;
        }

        public void Configure(UrlTrackerLegacyOptions options)
        {
            var urlTrackerOptionsValue = _urlTrackerOptions.CurrentValue;
            options.AppendPortNumber = urlTrackerOptionsValue.AppendPortNumber;
            options.BlockedUrlsList = urlTrackerOptionsValue.BlockedUrlsList;
            options.EnableLogging = urlTrackerOptionsValue.EnableLogging;
            options.IsDisabled = !urlTrackerOptionsValue.Enable;
            options.AllowedUserAgents = urlTrackerOptionsValue.AllowedUserAgents;
        }
    }
}
