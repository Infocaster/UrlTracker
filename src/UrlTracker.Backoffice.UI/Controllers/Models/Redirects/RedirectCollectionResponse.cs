using System.Collections.Generic;
using UrlTracker.Backoffice.UI.Controllers.Models.Base;

namespace UrlTracker.Backoffice.UI.Controllers.Models.Redirects
{
    internal class RedirectCollectionResponse
        : PagedCollectionResponseBase<RedirectResponse>
    {
        public RedirectCollectionResponse(IEnumerable<RedirectResponse> results, long total)
            : base(results, total)
        { }
    }
}
