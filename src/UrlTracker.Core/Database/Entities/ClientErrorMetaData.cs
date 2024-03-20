using System;

namespace UrlTracker.Core.Database.Entities
{
    public interface IClientErrorMetaData
    {
        string MostCommonReferrer { get; }
        DateTime? MostRecentOccurrance { get; }
        int? TotalOccurrances { get; }
        int ClientError { get; }
    }
    public class ClientErrorMetaData
        : IClientErrorMetaData
    {
        public string MostCommonReferrer { get; set; }

        public DateTime? MostRecentOccurrance { get; set; }

        public int? TotalOccurrances { get; set; }

        public int ClientError { get; set; }
    }
}