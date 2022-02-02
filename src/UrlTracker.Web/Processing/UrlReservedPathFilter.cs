using System;
using System.Linq;
using System.Threading.Tasks;
using UrlTracker.Core.Configuration;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Web.Configuration.Models;
using ILogger = UrlTracker.Core.Logging.ILogger;

namespace UrlTracker.Web.Processing
{
    public class UrlReservedPathFilter
        : IRequestInterceptFilter
    {
        private readonly IConfiguration<ReservedPathSettings> _configuration;
        private readonly ILogger _logger;

        public UrlReservedPathFilter(IConfiguration<ReservedPathSettings> configuration, ILogger logger)
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
            _logger.LogStart<UrlReservedPathFilter>();

            // unify path for easy comparison
            var path = url.Path.Trim('/') + '/';

            // configuration ensures that paths have the format 'my/url.aspx/'
            //    the current path also ensures this, so now we can simply compare the strings
            if (_configuration.Value.Paths.Any(p => path.StartsWith(p, StringComparison.InvariantCultureIgnoreCase)))
            {
                _logger.LogPathIsReserved<UrlReservedPathFilter>();
                return false;
            }

            return true;
        }
    }
}
