using System;
using System.Collections.Generic;

namespace UrlTracker.Core.Database.Models;


public class RecommendationOrderingOptions
{
    public bool Desc { get; set; } = true;
    public RecommendationOrderBy OrderBy { get; set; }
}

public class RecommendationFilterOptions
{
    public string? Query { get; set; }
    public IEnumerable<Guid>? Types { get; set; }
}

public enum RecommendationOrderBy
{
    Importance,
    MostRecentlyUpdated,
    Url
}
public static class EnumExtensions
{
    public static string GetDatabaseFieldName(this RecommendationOrderBy orderBy)
    {
        return orderBy switch
        {
            RecommendationOrderBy.MostRecentlyUpdated => "[updateDate]",
            RecommendationOrderBy.Url => "[url]",
            _ => "orderscore",
        };
    }
}
