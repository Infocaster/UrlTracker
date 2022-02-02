using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UrlTracker.Core.Domain.Models;

namespace UrlTracker.Core.Intercepting
{
    [ExcludeFromCodeCoverage]
    public class StaticUrlProvider
        : IStaticUrlProvider
    {
        public IEnumerable<string> Get(Url url)
        {
            return new[]
            {
                url.ToString(UrlType.Absolute, true, true),
                url.ToString(UrlType.Absolute, true, false),
                url.ToString(UrlType.Absolute, false, true),
                url.ToString(UrlType.Absolute, false, false),
                url.ToString(UrlType.Relative, true, true),
                url.ToString(UrlType.Relative, true, false),
                url.ToString(UrlType.Relative, false, true),
                url.ToString(UrlType.Relative, false, false),
            };
        }
    }
}
