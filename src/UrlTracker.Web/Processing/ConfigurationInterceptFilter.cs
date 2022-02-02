using System.Threading.Tasks;
using UrlTracker.Core.Configuration;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Core.Domain.Models;
using ILogger = UrlTracker.Core.Logging.ILogger;

namespace UrlTracker.Web.Processing
{
    public class ConfigurationInterceptFilter
        : IRequestInterceptFilter
    {
        private readonly IConfiguration<UrlTrackerSettings> _configuration;
        private readonly ILogger _logger;

        public ConfigurationInterceptFilter(IConfiguration<UrlTrackerSettings> configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public ValueTask<bool> EvaluateCandidateAsync(Url url)
        {
            return new ValueTask<bool>(EvaluateCandidate());
        }

        private bool EvaluateCandidate()
        {
            _logger.LogStart<ConfigurationInterceptFilter>();
            if (_configuration.Value.IsDisabled)
            {
                _logger.LogUrlTrackerDisabled<ConfigurationInterceptFilter>();
                return false;
            }

            return true;
        }
    }
}
