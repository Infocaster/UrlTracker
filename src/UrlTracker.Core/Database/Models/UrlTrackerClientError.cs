using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace UrlTracker.Core.Database.Models
{
    [ExcludeFromCodeCoverage]
    public class UrlTrackerShallowClientError
    {
        public UrlTrackerShallowClientError(HttpStatusCode targetStatusCode)
        {
            TargetStatusCode = targetStatusCode;
        }

        public int? Id { get; set; }
        public HttpStatusCode TargetStatusCode { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class UrlTrackerClientError
        : UrlTrackerShallowClientError
    {
        public UrlTrackerClientError(string url, HttpStatusCode targetStatusCode)
            : base(targetStatusCode)
        {
            Url = url;
        }

        public string? Culture { get; set; }
        public string Url { get; set; }
        public int? RootNodeId { get; set; }
        public int? NodeId { get; set; }
        public DateTime? Inserted { get; set; }
    }
}
