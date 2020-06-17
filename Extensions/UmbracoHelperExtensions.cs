using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using InfoCaster.Umbraco.UrlTracker.Models;
using Umbraco.Core.IO;
using Umbraco.Core.Composing;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Configuration;
using GlobalSettings = Umbraco.Core.Configuration.GlobalSettings;
using Umbraco.Web;

namespace InfoCaster.Umbraco.UrlTracker.Extensions
{
    public static class UmbracoHelperExtensions
    {
        static readonly object _locker = new object();
        static List<UrlTrackerDomain> _urlTrackerDomains;

        static volatile string _reservedUrlsCache;
        static string _reservedPathsCache;
        static List<string> _reservedList;
        public static List<UrlTrackerDomain> GetDomains(this UmbracoHelper umbracoHelper)
        {
            if (_urlTrackerDomains == null)
            {
                lock (_locker)
                {
                    _urlTrackerDomains = new List<UrlTrackerDomain>();
                    IEnumerable<IDomain> umbDomains = Current.Services.DomainService.GetAll(UrlTrackerSettings.HasDomainOnChildNode);

                    foreach (IDomain umbDomain in umbDomains.Where(ud => ud.RootContentId.HasValue))
                    {
                        _urlTrackerDomains.Add(new UrlTrackerDomain(umbDomain.Id, umbDomain.RootContentId.Value, umbDomain.DomainName));
                    }

                    _urlTrackerDomains = _urlTrackerDomains.OrderBy(x => x.Name).ToList();
                }
            }
            return _urlTrackerDomains;
        }

        public static void ClearDomains(this UmbracoHelper umbracoHelper)
        {
            lock (_locker)
            {
                _urlTrackerDomains = null;
            }
        }

        public static bool IsReservedPathOrUrl(this UmbracoHelper umbracoHelper, string url)
        {
            if (_reservedUrlsCache == null)
            {
                lock (_locker)
                {
                    if (_reservedUrlsCache == null)
                    {
                        // store references to strings to determine changes
                        
                        _reservedPathsCache = Current.Configs.Global().ReservedPaths; 
                        _reservedUrlsCache = Current.Configs.Global().ReservedUrls;

                        // add URLs and paths to a new list
                        List<string> _newReservedList = new List<string>();

                        foreach (string reservedUrl in _reservedUrlsCache.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (String.IsNullOrWhiteSpace(reservedUrl)) continue;

                            //resolves the url to support tilde chars
                            string reservedUrlTrimmed = IOHelper.ResolveUrl(reservedUrl).Trim().ToLower();
                            if (reservedUrlTrimmed.Length > 0)
                                _newReservedList.Add(reservedUrlTrimmed);
                        }

                        foreach (string reservedPath in _reservedPathsCache.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (String.IsNullOrWhiteSpace(reservedPath)) continue;

                            bool trimEnd = !reservedPath.EndsWith("/");
                            //resolves the url to support tilde chars
                            string reservedPathTrimmed = IOHelper.ResolveUrl(reservedPath).Trim().ToLower();

                            if (reservedPathTrimmed.Length > 0)
                                _newReservedList.Add(reservedPathTrimmed + (reservedPathTrimmed.EndsWith("/") ? "" : "/"));
                        }

                        // use the new list from now on
                        _reservedList = _newReservedList;
                    }
                }
            }
            //The url should be cleaned up before checking:
            // * If it doesn't contain an '.' in the path then we assume it is a path based URL, if that is the case we should add an trailing '/' because all of our reservedPaths use a trailing '/'
            // * We shouldn't be comparing the query at all
            var pathPart = url.Split('?')[0];
            if (!pathPart.Contains(".") && !pathPart.EndsWith("/"))
                pathPart += "/";

            // check if path is longer than one character, then if it does not start with / then add a /
            if (pathPart.Length > 1 && pathPart[0] != '/')
            {
                pathPart = '/' + pathPart; // fix because sometimes there is no leading /... depends on browser...
            }

            // return true if url starts with an element of the reserved list
            return _reservedList.Any(u => u.StartsWith(pathPart.ToLowerInvariant()));
        }

        public static string GetUrl(this UmbracoHelper umbracoHelper,int nodeId)
        {
            return umbracoHelper.Content(nodeId).Url;
        }

    }
}