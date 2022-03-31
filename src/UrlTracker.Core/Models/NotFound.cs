using System;
using System.ComponentModel.DataAnnotations;

namespace UrlTracker.Core.Models
{
    public class NotFound
    {
        public NotFound(string url)
        {
            Url = url;
        }

        public int? Id { get; set; }
        [Required]
        public string Url { get; set; }
        public string? Referrer { get; set; }
        public DateTime Inserted { get; set; }
        public bool Ignored { get; set; }
    }

    public class RichNotFound
    {
        public RichNotFound(string url)
        {
            Url = url;
        }

        public int Id { get; set; }
        public string Url { get; set; }
        public DateTime LatestOccurrence { get; set; }
        public string? MostCommonReferrer { get; set; }
        public int Occurrences { get; set; }
    }
}
