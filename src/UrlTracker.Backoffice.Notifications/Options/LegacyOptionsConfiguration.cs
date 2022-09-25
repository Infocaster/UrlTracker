using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;
using UrlTracker.Modules.Options;

namespace UrlTracker.Backoffice.Notifications.Options
{
    [ExcludeFromCodeCoverage]
    internal class LegacyOptionsConfiguration : IConfigureOptions<UrlTrackerLegacyOptions>
    {
        private readonly IOptionsMonitor<UrlTrackerNotificationsOptions> _notificationsOptions;

        public LegacyOptionsConfiguration(IOptionsMonitor<UrlTrackerNotificationsOptions> notificationsOptions)
        {
            _notificationsOptions = notificationsOptions;
        }

        public void Configure(UrlTrackerLegacyOptions options)
        {
            var notificationsOptionsValue = _notificationsOptions.CurrentValue;
            options.TrackingDisabled = !notificationsOptionsValue.Enable;
        }
    }
}
