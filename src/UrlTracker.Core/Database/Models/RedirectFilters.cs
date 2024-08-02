using System;
using System.Collections.Generic;
using UrlTracker.Core.Database;

namespace UrlTracker.Core.Database.Models
{
    public record RedirectFilters(bool Advanced, RedirectType Types, IEnumerable<Guid>? SourceTypes);
}