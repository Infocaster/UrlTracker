using System.Collections.Generic;

namespace UrlTracker.Web.Controllers.Models
{
    public class GetNotFoundsResponse
    {
        public IReadOnlyCollection<RedirectViewModel> Entries { get; set; }
        public int NumberOfEntries { get; set; }
    }
}
