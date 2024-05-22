using System;
using System.Collections.Generic;
using UrlTracker.Backoffice.UI.Controllers.Models.Base;
using UrlTracker.Core.Database;

namespace UrlTracker.Backoffice.UI.Controllers.Models.Redirects
{
    internal class ListRedirectRequest
        : PaginationRequest
    {
        public string? Query { get; set; }
        public IEnumerable<RedirectType>? Types { get; set; }
        public IEnumerable<Guid>? SourceTypes { get; set; }
    }
}
