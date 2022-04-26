using System.Linq;
using System.Threading.Tasks;
using UrlTracker.Core;
using UrlTracker.Core.Configuration;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Web.Events.Models;

namespace UrlTracker.Web.Processing
{
    public class ConfigurationClientErrorFilter
        : IClientErrorFilter
    {
        private readonly IConfiguration<UrlTrackerSettings> _configuration;

        public ConfigurationClientErrorFilter(IConfiguration<UrlTrackerSettings> configuration)
        {
            _configuration = configuration;
        }

        public ValueTask<bool> EvaluateCandidateAsync(ProcessedEventArgs e)
            => new ValueTask<bool>(EvaluateCandidate(e));

        private bool EvaluateCandidate(ProcessedEventArgs e)
        {
            // all 404 tracking should be cancelled if not found tracking or the entire URL Tracker is disabled
            var configurationValue = _configuration.Value;
            if (configurationValue.IsDisabled ||
                configurationValue.IsNotFoundTrackingDisabled) return false;

            // absolute path starts with /, so patterns should also take that into account
            if (Defaults.Tracking.IgnoredUrlPaths.Any(rx
                => rx.IsMatch(e.Url.Path))) return false;

            return true;
        }
    }
}
