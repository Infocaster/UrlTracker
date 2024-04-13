namespace UrlTracker.Core.Database.Models;


public class RecommendationOrderingOptions
{
    public bool Desc { get; set; } = true;
    public RecommendationOrderBy OrderBy { get; set; }
}

public enum RecommendationOrderBy
{
    Importance,
    LastOccurrence,
    Url,
    Occurrences
}
public static class EnumExtensions
{
    public static string GetDatabaseFieldName(this RecommendationOrderBy orderBy)
    {
        return orderBy switch
        {
            RecommendationOrderBy.LastOccurrence => "[updateDate]",
            RecommendationOrderBy.Url => "[url]",
            _ => "orderscore",
        };
    }
}
