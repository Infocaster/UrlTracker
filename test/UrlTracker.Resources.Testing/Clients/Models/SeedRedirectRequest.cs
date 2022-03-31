using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UrlTracker.Resources.Testing.Clients.Models
{
    public class SeedRedirectRequest
    {
        [Required]
        public ICollection<SeedRedirectRequestRedirect>? Redirects { get; set; }
    }

    public class SeedRedirectRequestRedirect
    {
        public int? Id { get; set; }
        public string? Culture { get; set; }
        public int? TargetRootNodeId { get; set; }
        public int? TargetNodeId { get; set; }
        public string? TargetUrl { get; set; }
        public string? SourceUrl { get; set; }
        public string? SourceRegex { get; set; }
        public bool PassThroughQueryString { get; set; }
        public string? Notes { get; set; }
        public int TargetStatusCode { get; set; }
        public bool Force { get; set; }
    }
}