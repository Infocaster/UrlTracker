using System.Diagnostics.CodeAnalysis;
using CsvHelper.Configuration.Attributes;

namespace UrlTracker.Backoffice.UI.Controllers.Models.RedirectImport
{
    [ExcludeFromCodeCoverage]
    public class CsvRedirect
    {
        [Name("RootNodeId"), Index(0)]
        public int? TargetRootNodeId { get; set; }

        [Name("Culture"), Index(1)]
        public string? Culture { get; set; }

        [Name("Old URL"), Index(2)]
        public string? SourceUrl { get; set; }

        [Name("Regex"), Index(3)]
        public string? SourceRegex { get; set; }

        [Name("Redirect URL"), Index(4)]
        public string? TargetUrl { get; set; }

        [Name("Redirect node ID"), Index(5)]
        public int? TargetNodeId { get; set; }

        [Name("Redirect HTTP Code"), Index(6)]
        public int TargetStatusCode { get; set; }

        [Name("Forward query"), Index(7)]
        public bool PassThroughQueryString { get; set; }

        [Name("Force redirect"), Index(8)]
        public bool Force { get; set; }

        [Name("Notes"), Index(9)]
        public string? Notes { get; set; }
    }
}
