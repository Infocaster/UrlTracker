using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UrlTracker.Core.Models;

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
                url.ToString(UrlType.Relative, true, true).TrimStart('/'),
                url.ToString(UrlType.Relative, true, false).TrimStart('/'),
                url.ToString(UrlType.Relative, false, true).TrimStart('/'),
                url.ToString(UrlType.Relative, false, false).TrimStart('/'),
            }.Where(u => !string.IsNullOrWhiteSpace(u));
        }
    }
}
