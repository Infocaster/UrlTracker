using System.Collections.Generic;
using System.Linq;
using UrlTracker.Backoffice.UI.Controllers.Models.Base;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Backoffice.UI.Controllers.Models.Redirects
{
    internal class RedirectCollectionResponse
        : PagedCollectionResponseBase<RedirectResponse>
    {
        public RedirectCollectionResponse(IEnumerable<RedirectResponse> results, long total)
            : base(results, total)
        { }

        public static RedirectCollectionResponse FromEntityCollection(RedirectEntityCollection entityCollection)
            => new(entityCollection.Select(RedirectResponse.FromEntity), entityCollection.Total);
    }
}
