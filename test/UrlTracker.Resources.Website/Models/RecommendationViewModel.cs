using System;

namespace UrlTracker.Resources.Website.Models
{
    public class RecommendationViewModel
    {
        public string Url { get; set; }
        public Guid Strategy { get; set; }
        public int VariableScore { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
