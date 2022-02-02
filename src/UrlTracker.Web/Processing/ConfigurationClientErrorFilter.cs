using System.Linq;
using System.Threading.Tasks;
using System.Web;
using UrlTracker.Core;
using UrlTracker.Core.Configuration;
using UrlTracker.Core.Configuration.Models;

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

        public ValueTask<bool> EvaluateCandidateAsync(HttpContextBase httpContext)
            => new ValueTask<bool>(EvaluateCandidate(httpContext));

        private bool EvaluateCandidate(HttpContextBase httpContext)
        {
            // all 404 tracking should be cancelled if not found tracking or the entire URL Tracker is disabled
            var configurationValue = _configuration.Value;
            if (configurationValue.IsDisabled ||
                configurationValue.IsNotFoundTrackingDisabled) return false;

            // absolute path starts with /, so patterns should also take that into account
            if (Defaults.Tracking.IgnoredUrlPaths.Any(rx
                => rx.IsMatch(httpContext.Request.Url.AbsolutePath))) return false;

            return true;
        }
    }
}
