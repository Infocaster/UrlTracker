using System;

namespace UrlTracker.Core.Database.Entities
{
    public interface IClientErrorMetaData
    {
        string? MostCommonReferrer { get; init; }
        DateTime? MostRecentOccurrance { get; init; }
        int? TotalOccurrances { get; init; }
        int ClientError { get; init; }
    }

    public record ClientErrorMetaData(
        int? TotalOccurrances,
        string? MostCommonReferrer,
        DateTime? MostRecentOccurrance,
        int ClientError
        ) : IClientErrorMetaData;
}
