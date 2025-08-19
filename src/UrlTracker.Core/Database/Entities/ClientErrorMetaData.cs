using System;

namespace UrlTracker.Core.Database.Entities
{
    public interface IClientErrorMetaData
    {
        string MostCommonReferrer { get; }
        DateTime? MostRecentOccurrance { get; }
        int? TotalOccurrences { get; }
        int ClientError { get; }
    }
    public class ClientErrorMetaData
        : IClientErrorMetaData
    {
        public string MostCommonReferrer { get; set; }

        public DateTime? MostRecentOccurrance { get; set; }

        public int? TotalOccurrences { get; set; }

        public int ClientError { get; set; }
    }
}