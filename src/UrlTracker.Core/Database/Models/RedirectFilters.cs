using System;
using System.Collections.Generic;
using UrlTracker.Core.Database;

public record RedirectFilters(RedirectType Types, IEnumerable<Guid>? SourceTypes);