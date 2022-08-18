using System.Collections.Generic;
using UrlTracker.Core.Database.Models.Entities;

namespace UrlTracker.Core.Caching
{
    public interface IRegexRedirectCache
        : ITypedMemoryCache<string, IReadOnlyCollection<IRedirect>>
    { }
}