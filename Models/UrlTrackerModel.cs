using InfoCaster.Umbraco.UrlTracker.Helpers;
using InfoCaster.Umbraco.UrlTracker.Services;
using InfoCaster.Umbraco.UrlTracker.Settings;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace InfoCaster.Umbraco.UrlTracker.Models
{
	[DebuggerDisplay("OUrl = {OldUrl} | Rgx = {OldRegex} | Qs = {OldUrlQuery} | Root = {RedirectRootNodeId}")]
	public class UrlTrackerModel
	{
		private IUrlTrackerHelper _urlTrackerHelper => DependencyResolver.Current.GetService<IUrlTrackerHelper>();
		private IUrlTrackerService _urlTrackerService => DependencyResolver.Current.GetService<IUrlTrackerService>();
		private IUrlTrackerSettings _urlTrackerSettings => DependencyResolver.Current.GetService<IUrlTrackerSettings>();

		private Lazy<string> _calculatedOldUrl => new Lazy<string>(() =>
		{
			if (!string.IsNullOrEmpty(CalculatedOldUrlWithDomain))
			{
				if (CalculatedOldUrlWithDomain.StartsWith("Regex:"))
					return CalculatedOldUrlWithDomain;

				Uri calculatedOldUrlWithDomain = new Uri(CalculatedOldUrlWithDomain);
				string pathAndQuery = Uri.UnescapeDataString(calculatedOldUrlWithDomain.PathAndQuery);

				return !pathAndQuery.StartsWith("/") ? string.Concat("/", pathAndQuery.Substring(1)) : pathAndQuery;
			}

			return string.Empty;
		});
		private Lazy<string> _calculatedOldUrlWithDomain => new Lazy<string>(() =>
		{
			try
			{
				if (string.IsNullOrEmpty(OldRegex) && string.IsNullOrEmpty(OldUrl))
					throw new InvalidOperationException("Both OldRegex and OldUrl are empty, which is invalid. Please correct this by removing any entries where the OldUrl and OldRegex columns are empty.");
				if (!string.IsNullOrEmpty(OldRegex) && string.IsNullOrEmpty(OldUrl))
					return string.Concat("Regex: ", OldRegex);

				var domain = _urlTrackerService.GetDomains()
					.FirstOrDefault(x => x.NodeId == RedirectRootNodeId);

				if (domain == null)
				{
					domain = new UrlTrackerDomain
					{
						Id = -1,
						NodeId = RedirectRootNodeId,
						Name = string.Concat(HttpContext.Current.Request.Url.Host,
							HttpContext.Current.Request.Url.IsDefaultPort && !_urlTrackerSettings.AppendPortNumber()
								? string.Empty
								: string.Concat(":", HttpContext.Current.Request.Url.Port))
					};
				}

				Uri domainUri = new Uri(domain.UrlWithDomain);
				string domainOnly = string.Format("{0}{1}{2}{3}", domainUri.Scheme, Uri.SchemeDelimiter, domainUri.Host, domainUri.IsDefaultPort && !_urlTrackerSettings.AppendPortNumber() ? string.Empty : string.Concat(":", domainUri.Port));

				if (_urlTrackerSettings.HasDomainOnChildNode())
					return string.Format("{0}{1}{2}", new Uri(string.Concat(domain.UrlWithDomain, !domain.UrlWithDomain.EndsWith("/") && !OldUrl.StartsWith("/") ? "/" : string.Empty, _urlTrackerHelper.ResolveUmbracoUrl(OldUrl))), !string.IsNullOrEmpty(OldUrlQuery) ? "?" : string.Empty, OldUrlQuery);

				return string.Format("{0}{1}{2}", new Uri(string.Concat(domainOnly, !domainOnly.EndsWith("/") && !OldUrl.StartsWith("/") ? "/" : string.Empty, _urlTrackerHelper.ResolveUmbracoUrl(OldUrl))), !string.IsNullOrEmpty(OldUrlQuery) ? "?" : string.Empty, OldUrlQuery);
			}
			catch (Exception e)
			{
				return string.Empty;
			}
		});
		private Lazy<UrlTrackerNodeModel> _redirectRootNode => new Lazy<UrlTrackerNodeModel>(() =>
		{
			var redirectRootNode = _urlTrackerService.GetNodeById(RedirectRootNodeId);

			if (redirectRootNode != null)
			{
				if (redirectRootNode.Id == 0)
				{
					var rootNode = _urlTrackerService.GetNodeById(-1).Children.FirstOrDefault();
					if (rootNode != null && rootNode.Id > 0)
						redirectRootNode = rootNode;
				}

				return new UrlTrackerNodeModel
				{
					Id = RedirectRootNodeId,
					Url = redirectRootNode.Url,
					Name = redirectRootNode.Name,
					Parent = new UrlTrackerNodeModel
					{
						Id = redirectRootNode.Parent?.Id ?? 0,
						Name = redirectRootNode.Parent?.Name ?? "",
						Url = redirectRootNode.Parent?.Url ?? ""
					}
				};
			}

			return null;
		});
		private Lazy<string> _redirectRootNodeName => new Lazy<string>(() =>
		{
			if (RedirectNode != null)
			{
				if (_urlTrackerSettings.HasDomainOnChildNode())
					return RedirectRootNode.Parent == null
						? RedirectRootNode.Name
						: RedirectRootNode.Parent.Name + "/" + RedirectRootNode.Name;

				return RedirectRootNode.Name;
			}

			return string.Empty;
		});
		private Lazy<UrlTrackerNodeModel> _redirectNode => new Lazy<UrlTrackerNodeModel>(() =>
		{
			if (RedirectNodeId.HasValue)
			{
				var node = _urlTrackerService.GetNodeById(RedirectNodeId.Value);

				return new UrlTrackerNodeModel
				{
					Id = node.Id,
					Name = node.Name,
					Url = node.Url,
					Parent = new UrlTrackerNodeModel
					{
						Id = node.Parent?.Id ?? 0,
						Name = node.Parent?.Name ?? "",
						Url = node.Parent?.Url ?? ""
					}
				};
			}

			return null;
		});
		private Lazy<string> _oldUrlQuery => new Lazy<string>(() =>
		{
			if (OldUrl.Contains("?"))
				return OldUrl.Substring(OldUrl.IndexOf('?') + 1);

			return string.Empty;
		});
		private Lazy<string> _calculatedRedirectUrl => new Lazy<string>(() =>
		{
			string calculatedRedirectUri = !string.IsNullOrEmpty(RedirectUrl) ? RedirectUrl : null;

			if (!string.IsNullOrEmpty(calculatedRedirectUri))
			{
				if (calculatedRedirectUri.StartsWith(Uri.UriSchemeHttp + Uri.SchemeDelimiter) || calculatedRedirectUri.StartsWith(Uri.UriSchemeHttps + Uri.SchemeDelimiter))
					return calculatedRedirectUri;

				return _urlTrackerHelper.ResolveShortestUrl(calculatedRedirectUri);
			}

			if (RedirectRootNode != null && RedirectNodeId.HasValue && _urlTrackerService.GetUrlByNodeId(RedirectRootNode.Id, Culture).EndsWith("#")) //ToDo
			{
				var siteDomains = _urlTrackerService.GetDomains().Where(x => x.NodeId == RedirectRootNode.Id).ToList();
				var hosts = siteDomains.Select(n => new Uri(n.UrlWithDomain).Host).ToList();

				if (hosts.Count == 0)
					return _urlTrackerHelper.ResolveShortestUrl(_urlTrackerService.GetUrlByNodeId(RedirectNodeId.Value, Culture));

				var sourceUrl = new Uri(siteDomains.First().UrlWithDomain);
				var targetUrl = new Uri(sourceUrl, _urlTrackerService.GetUrlByNodeId(RedirectNodeId.Value, Culture));

				if (!targetUrl.Host.Equals(sourceUrl.Host, StringComparison.OrdinalIgnoreCase))
					return targetUrl.AbsoluteUri;

				if (hosts.Any(n => n.Equals(sourceUrl.Host, StringComparison.OrdinalIgnoreCase)))
				{
					string url = _urlTrackerService.GetUrlByNodeId(RedirectNodeId.Value, Culture);
					// if current host is for this site already... (so no unnessecary redirects to primary domain)
					if (url.StartsWith(Uri.UriSchemeHttp)) // if url is with domain, strip domain
					{
						var uri = new Uri(url);
						return _urlTrackerHelper.ResolveShortestUrl(uri.AbsolutePath + uri.Fragment);
					}

					return _urlTrackerHelper.ResolveShortestUrl(url);
				}

				var newUri = new Uri(sourceUrl, _urlTrackerService.GetUrlByNodeId(RedirectNodeId.Value, Culture));

				if (sourceUrl.Host != newUri.Host)
					return _urlTrackerHelper.ResolveShortestUrl(newUri.AbsoluteUri);

				return _urlTrackerHelper.ResolveShortestUrl(newUri.AbsolutePath + newUri.Fragment);
			}

			if (RedirectNodeId.HasValue)
				return _urlTrackerHelper.ResolveShortestUrl(_urlTrackerService.GetUrlByNodeId(RedirectNodeId.Value));

			return string.Empty;
		});
		private Lazy<string> _oldUrlWithoutQuery => new Lazy<string>(() =>
		{
			if (CalculatedOldUrl.StartsWith("Regex:"))
				return CalculatedOldUrl;

			return CalculatedOldUrl.Contains('?') ? CalculatedOldUrl.Substring(0, CalculatedOldUrl.IndexOf('?')) : CalculatedOldUrl;
		});

		#region Calculated properties

		[JsonIgnore] public string CalculatedOldUrl => _calculatedOldUrl.Value;
		[JsonIgnore] public string CalculatedOldUrlWithDomain => _calculatedOldUrlWithDomain.Value;
		[JsonIgnore] public UrlTrackerNodeModel RedirectRootNode => _redirectRootNode.Value;
		[JsonIgnore] public string RedirectRootNodeName => _redirectRootNodeName.Value;
		[JsonIgnore] public UrlTrackerNodeModel RedirectNode => _redirectNode.Value;
		[JsonIgnore] public string OldUrlQuery => _oldUrlQuery.Value;
		public string CalculatedRedirectUrl => _calculatedRedirectUrl.Value;
		public string OldUrlWithoutQuery => _oldUrlWithoutQuery.Value;

		#endregion

		#region Data fields
		public int Id { get; set; }
		public string Culture { get; set; }
		public string OldUrl { get; set; }
		public string OldRegex { get; set; }
		public int RedirectRootNodeId { get; set; }
		public int? RedirectNodeId { get; set; }
		public string RedirectUrl { get; set; }
		public int RedirectHttpCode { get; set; }
		public bool RedirectPassThroughQueryString { get; set; }
		public string Notes { get; set; }
		public bool Is404 { get; set; }
		public bool Remove404 { get; set; }
		public string Referrer { get; set; }
		public int? Occurrences { get; set; }
		public DateTime Inserted { get; set; }
		public bool ForceRedirect { get; set; }
		#endregion
	}
}