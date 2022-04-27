using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace UrlTracker.Core
{
    [ExcludeFromCodeCoverage]
    public static partial class Defaults
    {
        public static class Tracking
        {
            public static readonly Regex[] IgnoredUrlPaths = new Regex[]
            {
                new Regex(@"__browserLink\/requestData\/.*", RegexOptions.Compiled | RegexOptions.IgnoreCase),
                new Regex(@"[^/]/arterySignalR\/ping", RegexOptions.Compiled | RegexOptions.IgnoreCase),
                new Regex(@"^\/favicon\.ico$", RegexOptions.Compiled | RegexOptions.IgnoreCase),
                new Regex(@"^\/umbraco(\/.*)?", RegexOptions.Compiled | RegexOptions.IgnoreCase),
                new Regex(@"\.js(\.map)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase)
            };
        }
    }
}
