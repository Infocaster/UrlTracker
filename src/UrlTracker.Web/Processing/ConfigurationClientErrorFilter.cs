using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using UrlTracker.Core;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Web.Events.Models;

namespace UrlTracker.Web.Processing
{
    public class ConfigurationClientErrorFilter
        : IClientErrorFilter
    {
        private readonly IOptions<UrlTrackerSettings> _configuration;

        public ConfigurationClientErrorFilter(IOptions<UrlTrackerSettings> configuration)
        {
            _configuration = configuration;
        }

        public ValueTask<bool> EvaluateCandidateAsync(UrlTrackerHandled notification)
            => new(EvaluateCandidate(notification));

        private bool EvaluateCandidate(UrlTrackerHandled notification)
        {
            // all 404 tracking should be cancelled if not found tracking or the entire URL Tracker is disabled
            var configurationValue = _configuration.Value;
            if (configurationValue.IsDisabled ||
                configurationValue.IsNotFoundTrackingDisabled) return false;

            // absolute path starts with /, so patterns should also take that into account
            if (Defaults.Tracking.IgnoredUrlPaths.Any(rx
                => rx.IsMatch(notification.Url.Path!))) return false;

            return true;
        }
    }
}
