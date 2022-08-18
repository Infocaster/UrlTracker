using System;

namespace UrlTracker.Core.Models
{
    public class ClientError
    {
        public ClientError(string url)
        {
            Url = url;
            Strategy = Defaults.DatabaseSchema.ClientErrorStrategies.NotFound;
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
