using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace UrlTracker.Backoffice.UI.Controllers.Models
{
    [ExcludeFromCodeCoverage]
    public class GetNotFoundsResponse
    {
        public GetNotFoundsResponse(IReadOnlyCollection<RedirectViewModel> entries, int numberOfEntries)
        {
            Entries = entries;
            NumberOfEntries = numberOfEntries;
        }

        public IReadOnlyCollection<RedirectViewModel> Entries { get; set; }
        public int NumberOfEntries { get; set; }
    }
}
