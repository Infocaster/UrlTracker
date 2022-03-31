using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace UrlTracker.Core.Database.Models
{
    [ExcludeFromCodeCoverage]
    public class UrlTrackerShallowRedirect
    {
        public int? Id { get; set; }
        public string? Culture { get; set; }
        public int? TargetRootNodeId { get; set; }
        public int? TargetNodeId { get; set; }
        public string? TargetUrl { get; set; }
        public string? SourceUrl { get; set; }
        public string? SourceRegex { get; set; }
        public bool PassThroughQueryString { get; set; }
        public HttpStatusCode TargetStatusCode { get; set; }
        public bool Force { get; set; }
    }

    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    [ExcludeFromCodeCoverage]
    public class UrlTrackerRedirect
        : UrlTrackerShallowRedirect
    {
        public string? Notes { get; set; }
        public DateTime Inserted { get; set; }

        private string GetDebuggerDisplay()
        {
            return ToString();
        }

        public override string ToString()
        {
            return $"{Id} | {Culture} | {TargetNodeId?.ToString() ?? TargetUrl} | {TargetStatusCode}";
        }
    }
}
