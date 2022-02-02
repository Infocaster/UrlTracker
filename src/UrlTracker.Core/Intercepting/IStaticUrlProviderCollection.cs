using System.Collections.Generic;
using UrlTracker.Core.Domain.Models;

namespace UrlTracker.Core.Intercepting
{
    public interface IStaticUrlProviderCollection
    {
        IEnumerable<string> GetUrls(Url url);
    }
}