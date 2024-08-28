using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using UAParser;
using UrlTracker.Core.Configuration.Models;

namespace UrlTracker.Web.Processing.Filtering
{
    public class UserAgentClientErrorFilter : IClientErrorFilter
    {
        private readonly IOptionsMonitor<UrlTrackerSettings> _options;
        private Parser _uaParser;

        public UserAgentClientErrorFilter(IOptionsMonitor<UrlTrackerSettings> options)
        {
            _options = options;
        }

        public ValueTask<bool> EvaluateCandidateAsync(HttpContext context)
            => ValueTask.FromResult(EvaluateCandidate(context));

        private bool EvaluateCandidate(HttpContext context)
        {
            var optionsValue = _options.CurrentValue;
            if (!optionsValue.AllowedUserAgents.Any()) return true;

            if (_uaParser is null) _uaParser = Parser.GetDefault();
            var userAgent = context.Request.Headers["User-Agent"].ToString();
            var clientInfo = _uaParser.ParseUserAgent(userAgent);

            foreach (var value in optionsValue.AllowedUserAgents)
            {
                //check if the options include the user agent from the request.
                if (value.Contains(clientInfo.Family, StringComparison.InvariantCultureIgnoreCase)) return true;
            }

            return false;
        }
    }
}
