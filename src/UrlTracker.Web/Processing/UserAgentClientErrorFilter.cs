using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using UAParser;
using UrlTracker.Core.Configuration.Models;

namespace UrlTracker.Web.Processing
{
    public class UserAgentClientErrorFilter : IClientErrorFilter
    {
        private readonly IOptionsMonitor<UrlTrackerSettings> _options;

        public UserAgentClientErrorFilter(IOptionsMonitor<UrlTrackerSettings> options)
        {
            _options = options;
        }

        public ValueTask<bool> EvaluateCandidateAsync(HttpContext context)
            => new(EvaluateCandidate(context));

        private bool EvaluateCandidate(HttpContext context)
        {
            var optionsValue = _options.CurrentValue;
            if (!optionsValue.AllowedUserAgents.Any()) return true;

            var uaParser = Parser.GetDefault();
            var userAgent = context.Request.Headers["User-Agent"].ToString();
            var clientInfo = uaParser.ParseUserAgent(userAgent);

            //check if the options include the user agent from the request.
            if (optionsValue.AllowedUserAgents.Contains(clientInfo.Family, StringComparer.InvariantCultureIgnoreCase)) return true;

            return false;
        }
    }
}
