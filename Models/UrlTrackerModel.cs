using InfoCaster.Umbraco.UrlTracker.Extensions;
using InfoCaster.Umbraco.UrlTracker.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InfoCaster.Umbraco.UrlTracker.Services;
using InfoCaster.Umbraco.UrlTracker.Settings;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.Composing;

namespace InfoCaster.Umbraco.UrlTracker.Models
{
	public enum UrlTrackerViewTypes
	{
		Custom,
		Auto,
		NotFound
	}

	[DebuggerDisplay("OUrl = {OldUrl} | Rgx = {OldRegex} | Qs = {OldUrlQuery} | Root = {RedirectRootNodeId}")]
	public class UrlTrackerModel
	{
		[JsonIgnore]
		private IUrlTrackerNewHelper _urlTrackerHelper => DependencyResolver.Current.GetService<IUrlTrackerNewHelper>();
		[JsonIgnore]
		private IUrlTrackerService _urlTrackerService => DependencyResolver.Current.GetService<IUrlTrackerService>();
		[JsonIgnore]
		private IUrlTrackerNewSettings _urlTrackerSettings => DependencyResolver.Current.GetService<IUrlTrackerNewSettings>();

		#region Calculated properties
		[JsonIgnore]
		public string CalculatedOldUrl
		{
			get
			{
				if (CalculatedOldUrlWithDomain.StartsWith("Regex:"))
					return CalculatedOldUrlWithDomain;

				Uri calculatedOldUrlWithDomain = new Uri(CalculatedOldUrlWithDomain);
				string pathAndQuery = Uri.UnescapeDataString(calculatedOldUrlWithDomain.PathAndQuery);

				return !pathAndQuery.StartsWith("/") ? string.Concat("/", pathAndQuery.Substring(1)) : pathAndQuery;
			}
		}

		[JsonIgnore]
		public string CalculatedOldUrlWithDomain
		{
			get
			{
				if (string.IsNullOrEmpty(OldRegex) && string.IsNullOrEmpty(OldUrl))
					throw new InvalidOperationException("Both OldRegex and OldUrl are empty, which is invalid. Please correct this by removing any entries where the OldUrl and OldRegex columns are empty.");
				if (!string.IsNullOrEmpty(OldRegex) && string.IsNullOrEmpty(OldUrl))
					return string.Concat("Regex: ", OldRegex);

				var domain = _urlTrackerService.GetDomains().FirstOrDefault(x => x.NodeId == RedirectRootNodeId);

				if (domain == null)
					domain = new UrlTrackerDomain(-1, RedirectRootNodeId, string.Concat(HttpContext.Current.Request.Url.Host, HttpContext.Current.Request.Url.IsDefaultPort && !_urlTrackerSettings.AppendPortNumber() ? string.Empty : string.Concat(":", HttpContext.Current.Request.Url.Port)));

				Uri domainUri = new Uri(domain.UrlWithDomain);
				string domainOnly = string.Format("{0}{1}{2}{3}", domainUri.Scheme, Uri.SchemeDelimiter, domainUri.Host, domainUri.IsDefaultPort && !_urlTrackerSettings.AppendPortNumber() ? string.Empty : string.Concat(":", domainUri.Port));

				if (_urlTrackerSettings.HasDomainOnChildNode())
					return string.Format("{0}{1}{2}", new Uri(string.Concat(domain.UrlWithDomain, !domain.UrlWithDomain.EndsWith("/") && !OldUrl.StartsWith("/") ? "/" : string.Empty, _urlTrackerHelper.ResolveUmbracoUrl(OldUrl))), !string.IsNullOrEmpty(OldUrlQuery) ? "?" : string.Empty, OldUrlQuery);

				return string.Format("{0}{1}{2}", new Uri(string.Concat(domainOnly, !domainOnly.EndsWith("/") && !OldUrl.StartsWith("/") ? "/" : string.Empty, _urlTrackerHelper.ResolveUmbracoUrl(OldUrl))), !string.IsNullOrEmpty(OldUrlQuery) ? "?" : string.Empty, OldUrlQuery);
			}
		}

		[JsonIgnore]
		public IPublishedContent RedirectRootNode
		{
			get
			{
				var redirectRootNode = _urlTrackerService.GetNodeById(RedirectRootNodeId);

				if (redirectRootNode != null && redirectRootNode.Id == 0)
				{
					var rootNode = _urlTrackerService.GetNodeById(-1).Children.FirstOrDefault();
					if (rootNode != null && rootNode.Id > 0)
						redirectRootNode = rootNode;
				}

				return redirectRootNode;
			}
		}

		[JsonIgnore]
		public string RedirectRootNodeName
		{
			get
			{
				if (_urlTrackerSettings.HasDomainOnChildNode())
				{
					return RedirectRootNode.Parent == null
						? RedirectRootNode.Name
						: RedirectRootNode.Parent.Name + "/" + RedirectRootNode.Name;
				}
				else
				{
					return RedirectRootNode.Name;
				}
			}
		}

		[JsonIgnore]
		public UrlTrackerViewTypes ViewType
		{
			get
			{
				if (RedirectNodeId.HasValue && ((Notes.StartsWith("A parent") || Notes.StartsWith("An ancestor") || Notes.StartsWith("This page") || Notes.StartsWith("This document")) && (Notes.EndsWith(" was moved") || Notes.EndsWith(" was renamed") || Notes.EndsWith("'s property 'umbracoUrlName' changed"))))
					return UrlTrackerViewTypes.Auto;
				if (Is404)
					return UrlTrackerViewTypes.NotFound;
				return UrlTrackerViewTypes.Custom;
			}
		}

		[JsonIgnore]
		public IPublishedContent RedirectNode
		{
			get
			{
				if (RedirectNodeId.HasValue)
					return _urlTrackerService.GetNodeById(RedirectNodeId.Value);

				return null;
			}
		}

		[JsonIgnore]
		public bool RedirectNodeIsPublished
		{
			get
			{
				return RedirectNode?.IsPublished() ?? false;
			}
		}

		[JsonIgnore]
		public string OldUrlQuery
		{
			get
			{
				if (OldUrl.Contains("?"))
					return OldUrl.Substring(OldUrl.IndexOf('?') + 1);

				return "";
			}
		}

		[JsonIgnore]
		public string RedirectNodeName
		{
			get
			{
				return RedirectNode?.Name ?? "";
			}
		}

		public string CalculatedRedirectUrl
		{
			get
			{
				string calculatedRedirectUri = !string.IsNullOrEmpty(RedirectUrl) ? RedirectUrl : null;

				if (calculatedRedirectUri != null)
				{
					if (calculatedRedirectUri.StartsWith(Uri.UriSchemeHttp + Uri.SchemeDelimiter) || calculatedRedirectUri.StartsWith(Uri.UriSchemeHttps + Uri.SchemeDelimiter))
						return calculatedRedirectUri;

					return _urlTrackerHelper.ResolveShortestUrl(calculatedRedirectUri);
				}

				if (RedirectRootNode != null && !RedirectRootNode.Url.EndsWith("#") && RedirectNodeId.HasValue)
				{
					var siteDomains = _urlTrackerService.GetDomains().Where(x => x.NodeId == RedirectRootNode.Id).ToList();
					var hosts = siteDomains.Select(n => new Uri(n.UrlWithDomain).Host).ToList();

					if (hosts.Count == 0)
						return _urlTrackerHelper.ResolveShortestUrl(_urlTrackerService.GetUrlByNodeId(RedirectNodeId.Value));

					var sourceUrl = new Uri(siteDomains.First().UrlWithDomain);
					var targetUrl = new Uri(sourceUrl, _urlTrackerService.GetUrlByNodeId(RedirectNodeId.Value));

					if (!targetUrl.Host.Equals(sourceUrl.Host, StringComparison.OrdinalIgnoreCase))
						return targetUrl.AbsoluteUri;

					if (hosts.Any(n => n.Equals(sourceUrl.Host, StringComparison.OrdinalIgnoreCase)))
					{
						string url = _urlTrackerService.GetUrlByNodeId(RedirectNodeId.Value);
						// if current host is for this site already... (so no unnessecary redirects to primary domain)
						if (url.StartsWith(Uri.UriSchemeHttp)) // if url is with domain, strip domain
						{
							var uri = new Uri(url);
							return _urlTrackerHelper.ResolveShortestUrl(uri.AbsolutePath + uri.Fragment);
						}

						return _urlTrackerHelper.ResolveShortestUrl(url);
					}

					var newUri = new Uri(sourceUrl, _urlTrackerService.GetUrlByNodeId(RedirectNodeId.Value));

					if (sourceUrl.Host != newUri.Host)
						return _urlTrackerHelper.ResolveShortestUrl(newUri.AbsoluteUri);

					return _urlTrackerHelper.ResolveShortestUrl(newUri.AbsolutePath + newUri.Fragment);
				}

				if (RedirectNodeId.HasValue)
					return _urlTrackerHelper.ResolveShortestUrl(_urlTrackerService.GetUrlByNodeId(RedirectNodeId.Value));

				return string.Empty;
			}
		}

		public string OldUrlWithoutQuery
		{
			get
			{
				if (CalculatedOldUrl.StartsWith("Regex:"))
					return CalculatedOldUrl;
				return CalculatedOldUrl.Contains('?') ? CalculatedOldUrl.Substring(0, CalculatedOldUrl.IndexOf('?')) : CalculatedOldUrl;
			}
		}

		#endregion

		#region Data fields
		public int Id { get; set; }
		public string OldUrl { get; set; }
		public string OldRegex { get; set; }
		public int RedirectRootNodeId { get; set; }
		public int? RedirectNodeId { get; set; }
		public string RedirectUrl { get; set; }
		public int RedirectHttpCode { get; set; }
		public bool RedirectPassThroughQueryString { get; set; }
		public string Notes { get; set; }
		public bool Is404 { get; set; }
		public string Referrer { get; set; }
		public int? Occurrences { get; set; }
		public DateTime Inserted { get; set; }
		public bool ForceRedirect { get; set; }
		#endregion
	}
}