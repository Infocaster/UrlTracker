using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using InfoCaster.Umbraco.UrlTracker.Models;
using InfoCaster.Umbraco.UrlTracker.Services;
using InfoCaster.Umbraco.UrlTracker.Settings;
using Lucene.Net.Util;
using Umbraco.Core.Configuration;
using Umbraco.Core.IO;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace InfoCaster.Umbraco.UrlTracker.Helpers
{
	public class UrlTrackerNewHelper : IUrlTrackerNewHelper
	{
		private readonly IUrlTrackerCacheService _urlTrackerCacheService;
		private readonly IGlobalSettings _globalSettings;

		private readonly string _reservedListCacheKey = "UrlTrackerReservedList";

		public UrlTrackerNewHelper(IUrlTrackerCacheService urlTrackerCacheService, IGlobalSettings globalSettings)
		{
			_urlTrackerCacheService = urlTrackerCacheService;
			_globalSettings = globalSettings;
		}

		public string ResolveShortestUrl(string url)
		{
			if (string.IsNullOrEmpty(url))
				return url;

			if (url.StartsWith("http://") || url.StartsWith("https://"))
			{
				Uri uri = new Uri(url);
				url = Uri.UnescapeDataString(uri.PathAndQuery);
			}

			if (url != "/")
			{
				// The URL should be stored as short as possible (e.g.: /page.aspx -> page | /page/ -> page)
				if (url.StartsWith("/"))
					url = url.Substring(1);
				if (url.EndsWith("/"))
					url = url.Substring(0, url.Length - "/".Length);
			}

			return url;
		}

		public string ResolveUmbracoUrl(string url)
		{
			if (url.StartsWith("http://") || url.StartsWith("https://"))
			{
				Uri uri = new Uri(url);
				url = Uri.UnescapeDataString(uri.PathAndQuery);
			}

			return url;
		}

		public bool IsReservedPathOrUrl(string url)
		{
			var reservedList = _urlTrackerCacheService.Get<List<string>>(_reservedListCacheKey);

			if (reservedList == null)
			{
				reservedList = new List<string>();

				var reservedUrls = _globalSettings.ReservedUrls;
				var reservedPaths = _globalSettings.ReservedPaths;

				foreach (var reservedUrl in reservedUrls.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
				{
					if(string.IsNullOrWhiteSpace(reservedUrl))
						continue;

					//Resolves the url to support tilde chars
					string reservedUrlTrimmed = IOHelper.ResolveUrl(reservedUrl).Trim().ToLower();

					if (reservedUrlTrimmed.Length > 0)
						reservedList.Add(reservedUrlTrimmed);
				}

				foreach (string reservedPath in reservedPaths.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
				{
					if (string.IsNullOrWhiteSpace(reservedPath)) 
						continue;

					bool trimEnd = !reservedPath.EndsWith("/");

					//Resolves the url to support tilde chars
					string reservedPathTrimmed = IOHelper.ResolveUrl(reservedPath).Trim().ToLower();

					if (reservedPathTrimmed.Length > 0)
						reservedList.Add(reservedPathTrimmed + (reservedPathTrimmed.EndsWith("/") ? "" : "/"));
				}

				_urlTrackerCacheService.Set(_reservedListCacheKey, reservedList);
			}

			//The url should be cleaned up before checking:
			// * If it doesn't contain an '.' in the path then we assume it is a path based URL, if that is the case we should add an trailing '/' because all of our reservedPaths use a trailing '/'
			// * We shouldn't be comparing the query at all
			var pathPart = url.Split('?')[0];
			if (!pathPart.Contains(".") && !pathPart.EndsWith("/"))
				pathPart += "/";

			// check if path is longer than one character, then if it does not start with / then add a /
			if (pathPart.Length > 1 && pathPart[0] != '/')
				pathPart = '/' + pathPart; // fix because sometimes there is no leading /... depends on browser...

			return reservedList.Any(u => u.StartsWith(pathPart.ToLowerInvariant()));
		}
	}
}
