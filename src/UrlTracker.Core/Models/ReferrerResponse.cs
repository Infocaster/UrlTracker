using System;

namespace UrlTracker.Core.Models
{
    public class ReferrerResponse
    {
        public ReferrerResponse(int referrerOccurances, string referrerUrl)
        {
            ReferrerOccurances = referrerOccurances;
            ReferrerUrl = referrerUrl;
        }

        public int ReferrerOccurances { get; set; }

        public string ReferrerUrl { get; set; }
    }
}