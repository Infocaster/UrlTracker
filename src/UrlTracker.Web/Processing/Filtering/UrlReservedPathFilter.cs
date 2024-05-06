using System;
using System.Linq;
using System.Threading.Tasks;
using UrlTracker.Core.Logging;
using UrlTracker.Core.Models;
using UrlTracker.Web.Configuration;

namespace UrlTracker.Web.Processing.Filtering
{
    public class UrlReservedPathFilter
        : IRequestInterceptFilter
    {
        private readonly IReservedPathSettingsProvider _configuration;
        private readonly ILogger<UrlReservedPathFilter> _logger;

        public UrlReservedPathFilter(IReservedPathSettingsProvider configuration, ILogger<UrlReservedPathFilter> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public ValueTask<bool> EvaluateCandidateAsync(Url url)
        {
            return new ValueTask<bool>(EvaluateCandidate(url));
        }

        public bool EvaluateCandidate(Url url)
        {
            _logger.LogStart(typeof(UrlReservedPathFilter));

            // unify path for easy comparison
            var path = url.Path!.Trim('/') + '/';

            // configuration ensures that paths have the format 'my/url.aspx/'
            //    the current path also ensures this, so now we can simply compare the strings
            if (_configuration.Paths.Any(p => path.StartsWith(p, StringComparison.InvariantCultureIgnoreCase)))
            {
                _logger.LogPathIsReserved();
                return false;
            }

            return true;
        }
    }
}
