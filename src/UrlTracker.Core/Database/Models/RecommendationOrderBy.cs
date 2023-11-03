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
        switch (orderBy)
        {
            case RecommendationOrderBy.LastOccurrence:
                return "[updateDate]";
            case RecommendationOrderBy.Url:
                return "[url]";
            default:
                return "orderscore";
        }
    }
}
