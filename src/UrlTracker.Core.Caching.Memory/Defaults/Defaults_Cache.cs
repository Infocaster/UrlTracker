namespace UrlTracker.Core.Caching.Memory
{
    public static partial class Defaults
    {
        /// <summary>
        /// Constants related to caching
        /// </summary>
        public static class Cache
        {
            /// <summary>
            /// Cache key to use when caching umbraco domains
            /// </summary>
            public const string DomainKey = "ic:UrlTrackerDomains";

            /// <summary>
            /// Cache key to use when caching regex redirects
            /// </summary>
            public const string RegexRedirectKey = "ic:UrlTrackerGetShallowWithRegexAsync";
        }
    }
}
