using System;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Core.Models
{
    public class ClientError
    {
        public ClientError(string url)
        {
            Url = url;
            Strategy = Defaults.DatabaseSchema.ClientErrorStrategies.NotFound;
        }

        public ClientError(IClientError clientError, IClientErrorMetaData metaData)
            : this(clientError.Url)
        {
            Id = clientError.Id;
            Ignored = clientError.Ignored;
            Inserted = clientError.CreateDate;

            if (metaData != null)
            {
                LatestOccurrence = metaData.MostRecentOccurrance ?? default;
                MostCommonReferrer = metaData.MostCommonReferrer;
                Occurrences = metaData.TotalOccurrances ?? default;
            }
        }

        public int Id { get; set; }
        public string Url { get; set; }
        public bool Ignored { get; set; }
        public DateTime Inserted { get; set; }
        public DateTime LatestOccurrence { get; set; }
        public string MostCommonReferrer { get; set; }
        public int Occurrences { get; set; }
        public Guid Strategy { get; set; }
    }
}
