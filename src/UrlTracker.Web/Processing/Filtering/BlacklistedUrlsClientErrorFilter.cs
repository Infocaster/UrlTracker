using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using UrlTracker.Core.Configuration.Models;

namespace UrlTracker.Web.Processing.Filtering
{
    public class BlacklistedUrlsClientErrorFilter : IClientErrorFilter
    {
        private readonly IOptionsMonitor<UrlTrackerSettings> _options;

        public BlacklistedUrlsClientErrorFilter(IOptionsMonitor<UrlTrackerSettings> options)
        {
            _options = options;
        }

        public ValueTask<bool> EvaluateCandidateAsync(HttpContext context)
            => new(EvaluateCandidate(context));

        private bool EvaluateCandidate(HttpContext context)
        {
            var optionsValue = _options.CurrentValue;
            if (!optionsValue.BlockedUrlsList.Any()) return true;
            //get the host + path from the url
            var url = context.Request.GetUrl().ToString();

            //check if any item in blockedUrl contains the url. The foreach is used instead of a .Contains on the list to make sure that items in the list containing https / querystrings etc also get caught
            foreach (var blockedUrl in optionsValue.BlockedUrlsList)
            {
                if (url.Contains(blockedUrl, StringComparison.InvariantCultureIgnoreCase)) return false;
            }
            return true;
        }
    }
}
