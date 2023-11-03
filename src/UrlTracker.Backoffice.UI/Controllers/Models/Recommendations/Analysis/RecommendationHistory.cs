using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlTracker.Backoffice.UI.Controllers.Models.Recommendations.Analysis;

public class RecommendationHistory
{
    public DateTime FirstOccurance { get; set; }
    public DateTime LastOccurance { get; set; }
    public double AveragePerDay { get; set; }
    public RecomendationTrend Trend { get; set; } = RecomendationTrend.Unknown;
    public IEnumerable<DailyOccurance> DailyOccurances { get; set; } = new List<DailyOccurance>();
}

public class DailyOccurance
{
    public int Occurances { get; set; }
    public DateTime DateTime { get; set; }
}

public enum RecomendationTrend
{
    Unknown,
    Stable,
    Increasing,
    Decreasing
}