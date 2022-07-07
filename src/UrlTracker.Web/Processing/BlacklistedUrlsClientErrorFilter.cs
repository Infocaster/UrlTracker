using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Web.Events.Models;

namespace UrlTracker.Web.Processing
{
    public class BlacklistedUrlsClientErrorFilter : IClientErrorFilter
    {
        private readonly UrlTrackerSettings _options;

        public BlacklistedUrlsClientErrorFilter(IOptions<UrlTrackerSettings> options)
        {
            _options = options.Value;
        }

        public ValueTask<bool> EvaluateCandidateAsync(UrlTrackerHandled notification)
            => new ValueTask<bool>(EvaluateCandidate(notification));

        public bool EvaluateCandidate(UrlTrackerHandled notification)
        {
            if (!_options.BlockedUrlsList.Any()) return true;
            //get the host + path from the url
            var url = notification.Url.ToString();

            //check if any item in blockedUrl contains the url. The foreach is used instead of a .Contains on the list to make sure that items in the list containing https / querystrings etc also get caught
            foreach (var blockedUrl in _options.BlockedUrlsList)
            {
                if (url.Contains(blockedUrl, StringComparison.InvariantCultureIgnoreCase)) return false;
            }
            return true;
        }
    }
}
