using System;
using System.Linq;
using System.Threading.Tasks;
using UrlTracker.Core.Configuration;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Web.Events.Models;

namespace UrlTracker.Web.Processing
{
    public class BlacklistedUrlsClientErrorFilter : IClientErrorFilter
    {
        private readonly IConfiguration<UrlTrackerSettings> _configuration;

        public BlacklistedUrlsClientErrorFilter(IConfiguration<UrlTrackerSettings> configuration)
        {
            _configuration = configuration;
        }

        public ValueTask<bool> EvaluateCandidateAsync(ProcessedEventArgs e)
           => new ValueTask<bool>(EvaluateCandidate(e));

        private bool EvaluateCandidate(ProcessedEventArgs e)
        {
            var configuration = _configuration.Value;
            if (configuration.BlockedUrlsList.Count == 0) return true;
            //get the host + path from the url
            var url = e.Url.ToString();

            //check if any item in blockedUrl contains the url. The foreach is used instead of a .Contains on the list to make sure that items in the list containing https / querystrings etc also get caught
            foreach (var blockedUrl in configuration.BlockedUrlsList)
            {
                if (url.Contains(blockedUrl.Trim())) return false;
            }
            return true;
        }
    }
}
