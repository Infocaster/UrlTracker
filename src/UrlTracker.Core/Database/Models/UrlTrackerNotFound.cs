using System;
using System.Diagnostics.CodeAnalysis;

namespace UrlTracker.Core.Database.Models
{
    [ExcludeFromCodeCoverage]
    public class UrlTrackerNotFound
    {
        public UrlTrackerNotFound(string url)
        {
            Url = url;
        }

        public int? Id { get; set; }
        public string Url { get; set; }
        public string? Referrer { get; set; }
        public bool Ignored { get; set; }
        public DateTime Inserted { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class UrlTrackerRichNotFound
    {
        public UrlTrackerRichNotFound(string url)
        {
            Url = url;
        }

        public int? Id { get; set; }
        public string Url { get; set; }
        public string? MostCommonReferrer { get; set; }
        public DateTime LatestOccurrence { get; set; }
        public int Occurrences { get; set; }
    }
}
