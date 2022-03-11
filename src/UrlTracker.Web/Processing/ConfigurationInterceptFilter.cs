using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using UrlTracker.Core.Configuration;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Logging;

namespace UrlTracker.Web.Processing
{
    public class ConfigurationInterceptFilter
        : IRequestInterceptFilter
    {
        private readonly IOptions<UrlTrackerSettings> _configuration;
        private readonly ILogger<ConfigurationInterceptFilter> _logger;

        public ConfigurationInterceptFilter(IOptions<UrlTrackerSettings> configuration, ILogger<ConfigurationInterceptFilter> logger)
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
                _logger.LogUrlTrackerDisabled();
                return false;
            }

            return true;
        }
    }
}
