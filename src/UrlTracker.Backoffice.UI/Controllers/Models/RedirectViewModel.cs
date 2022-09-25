using System;

namespace UrlTracker.Backoffice.UI.Controllers.Models
{
    // ToDo: These values are taken straight from the old code, because the frontend may or may not rely on them.
    //    Check which values are actually required and which values can be discarded!
    public class RedirectViewModel
    {
        public string? CalculatedRedirectUrl { get; set; }
        public string? OldUrlWithoutQuery { get; set; }
        public int Id { get; set; }
        public string? Culture { get; set; }
        public string? OldUrl { get; set; }
        public string? OldRegex { get; set; }
        public int RedirectRootNodeId { get; set; }
        public int? RedirectNodeId { get; set; }
        public string? RedirectUrl { get; set; }
        public int RedirectHttpCode { get; set; }
        public bool RedirectPassThroughQueryString { get; set; }
        public string? Notes { get; set; }
        public bool Is404 { get; set; }
        public bool Remove404 { get; set; }
        public string? Referrer { get; set; }
        public int? Occurrences { get; set; }
        public DateTime Inserted { get; set; }
        public bool ForceRedirect { get; set; }
    }
}
