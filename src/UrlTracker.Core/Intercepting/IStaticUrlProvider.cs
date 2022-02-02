using System.Collections.Generic;
using UrlTracker.Core.Domain.Models;

namespace UrlTracker.Core.Intercepting
{
    public interface IStaticUrlProvider
    {
        IEnumerable<string> Get(Url url);
    }
}
