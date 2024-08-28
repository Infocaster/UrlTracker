using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UrlTracker.Backoffice.UI.Controllers.Models.Recommendations.Analysis;

[DataContract]
internal record RecommendationHistory(
    [property: DataMember(Name = "firstOccurance")] DateTime FirstOccurance,
    [property: DataMember(Name = "lastOccurance")] DateTime LastOccurance,
    [property: DataMember(Name = "averagePerDay")] double AveragePerDay,
    [property: DataMember(Name = "dailyOccurances")] IEnumerable<DailyOccurance> DailyOccurances,
    [property: DataMember(Name = "trend")] RecomendationTrend Trend = RecomendationTrend.Unknown);

[DataContract]
internal record DailyOccurance(
    [property: DataMember(Name = "occurances")] int Occurances,
    [property: DataMember(Name = "dateTime")] DateTime DateTime);

internal enum RecomendationTrend
{
    Unknown,
    Stable,
    Increasing,
    Decreasing
}