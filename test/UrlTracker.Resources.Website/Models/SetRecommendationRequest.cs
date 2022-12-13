using System;

namespace UrlTracker.Resources.Website.Models
{
    public class SetRecommendationRequest
    {
        public string Url { get; set; }
        public DateTime DateTime { get; set; }
        public int Visits { get; set; }
    }
}
