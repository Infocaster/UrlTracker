using UrlTracker.Backoffice.UI.Controllers.Models.Base;

namespace UrlTracker.Backoffice.UI.Controllers.Models.Redirects
{
    internal class ListRedirectRequest
        : PaginationRequest
    {
        public string? Query { get; set; }
    }
}
