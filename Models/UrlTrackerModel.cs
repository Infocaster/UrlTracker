using InfoCaster.Umbraco.UrlTracker.Extensions;
using InfoCaster.Umbraco.UrlTracker.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
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

    [DebuggerDisplay("OUrl = {OldUrl} | Rgx = {OldRegex} | Qs = {OldUrlQueryString} | Root = {RedirectRootNodeId}")]
    public class UrlTrackerModel
    {
        [JsonIgnore]
        private UmbracoHelper _umbracoHelper;
        #region Data fields
        public int Id { get; set; }
        public string OldUrl { get; set; }
        public string OldUrlQueryString { get; set; }
        public string OldRegex { get; set; }
        public int RedirectRootNodeId { get; set; }
        public int? RedirectNodeId { get; set; }
        public string RedirectUrl { get; set; }
        public int RedirectHttpCode { get; set; }
        public bool RedirectPassThroughQueryString { get; set; }
        public string Notes { get; set; }
        public bool Is404 { get; set; }
        public string Referrer { get; set; }
        public int? NotFoundCount { get; set; }
        public DateTime Inserted { get; set; }
        public bool ForceRedirect { get; set; }
        #endregion

        #region Calculated properties
        [JsonIgnore]
        public string CalculatedOldUrlWithoutQuery
        {
            get
            {
                if (CalculatedOldUrl.StartsWith("Regex:"))
                    return CalculatedOldUrl;
                return CalculatedOldUrl.Contains('?') ? CalculatedOldUrl.Substring(0, CalculatedOldUrl.IndexOf('?')) : CalculatedOldUrl;
            }
        }
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

                UrlTrackerDomain domain = null;
                List<UrlTrackerDomain> domains = _umbracoHelper.GetDomains();
                domain = domains.FirstOrDefault(x => x.NodeId == RedirectRootNode.Id);
                if (domain == null)
                    domain = new UrlTrackerDomain(-1, RedirectRootNode.Id, string.Concat(HttpContext.Current.Request.Url.Host, HttpContext.Current.Request.Url.IsDefaultPort && !UrlTrackerSettings.AppendPortNumber ? string.Empty : string.Concat(":", HttpContext.Current.Request.Url.Port)));

                Uri domainUri = new Uri(domain.UrlWithDomain);
                string domainOnly = string.Format("{0}{1}{2}{3}", domainUri.Scheme, Uri.SchemeDelimiter, domainUri.Host, domainUri.IsDefaultPort && !UrlTrackerSettings.AppendPortNumber ? string.Empty : string.Concat(":", domainUri.Port));

                if (UrlTrackerSettings.HasDomainOnChildNode)
                {
                    return string.Format("{0}{1}{2}", new Uri(string.Concat(domain.UrlWithDomain, !domain.UrlWithDomain.EndsWith("/") && !OldUrl.StartsWith("/") ? "/" : string.Empty, UrlTrackerHelper.ResolveUmbracoUrl(OldUrl))), !string.IsNullOrEmpty(OldUrlQueryString) ? "?" : string.Empty, OldUrlQueryString);
                }
                else
                {
                    return string.Format("{0}{1}{2}", new Uri(string.Concat(domainOnly, !domainOnly.EndsWith("/") && !OldUrl.StartsWith("/") ? "/" : string.Empty, UrlTrackerHelper.ResolveUmbracoUrl(OldUrl))), !string.IsNullOrEmpty(OldUrlQueryString) ? "?" : string.Empty, OldUrlQueryString);
                }
            }
        }
        [JsonIgnore]
        public string CalculatedRedirectUrl
        {
            get
            {
                /*string calculatedRedirectUrl = !string.IsNullOrEmpty(RedirectUrl) ?
                    RedirectUrl :
                    !RedirectRootNode.NiceUrl.EndsWith("#") && RedirectNodeId.HasValue ?
                        new Uri(umbraco.library.NiceUrl(RedirectNodeId.Value).StartsWith("http") ? umbraco.library.NiceUrl(RedirectNodeId.Value) :
                            string.Format("{0}://{1}{2}{3}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Host, HttpContext.Current.Request.Url.Port != 80 && UrlTrackerSettings.AppendPortNumber ?
                                string.Concat(":", HttpContext.Current.Request.Url.Port) :
                                string.Empty, umbraco.library.NiceUrl(RedirectNodeId.Value)
                            )
                        ).AbsolutePath :
                        string.Empty;
                return !calculatedRedirectUrl.StartsWith("/") ? string.Concat("/", calculatedRedirectUrl) : calculatedRedirectUrl;
                 * */
                string calculatedRedirectUri = !string.IsNullOrEmpty(RedirectUrl)?RedirectUrl:null;
                if (calculatedRedirectUri != null)
                {
                    if (calculatedRedirectUri.StartsWith(Uri.UriSchemeHttp + Uri.SchemeDelimiter) || calculatedRedirectUri.StartsWith(Uri.UriSchemeHttps + Uri.SchemeDelimiter))
                    {
                        return calculatedRedirectUri;
                    }
                    else
                    {
                        return !calculatedRedirectUri.StartsWith("/") ? string.Concat("/", calculatedRedirectUri) : calculatedRedirectUri;
                    }
                }

                if (!RedirectRootNode.Url.EndsWith("#") && RedirectNodeId.HasValue)
                {

                    List<UrlTrackerDomain> domains = _umbracoHelper.GetDomains();
                    List<UrlTrackerDomain> siteDomains = domains.Where(x => x.NodeId == RedirectRootNode.Id).ToList();
                    List<string> hosts =
                        siteDomains
                        .Select(n => new Uri(n.UrlWithDomain).Host)
                        .ToList();
                    if (hosts.Count == 0)
                    {
                        return _umbracoHelper.GetUrl(RedirectNodeId.Value);
                    }
                    var sourceUrl = new Uri(siteDomains.First().UrlWithDomain);
                    var targetUrl = new Uri(sourceUrl,_umbracoHelper.GetUrl(RedirectNodeId.Value));
                    if (!targetUrl.Host.Equals(sourceUrl.Host, StringComparison.OrdinalIgnoreCase))
                    {
                        return targetUrl.AbsoluteUri;
                    }
                    if (hosts.Any(n => n.Equals(sourceUrl.Host, StringComparison.OrdinalIgnoreCase)))
                    {
                        string url = _umbracoHelper.GetUrl(RedirectNodeId.Value);
                        // if current host is for this site already... (so no unnessecary redirects to primary domain)
                        if (url.StartsWith(Uri.UriSchemeHttp))
                        {
                            // if url is with domain, strip domain
                            var uri = new Uri(url);
                            return uri.AbsolutePath + uri.Fragment;
                        }
                        return url;
                    }
                    var newUri = new Uri(sourceUrl, _umbracoHelper.GetUrl(RedirectNodeId.Value));
                    if (sourceUrl.Host != newUri.Host)
                    {
                        return newUri.AbsoluteUri;
                    }
                    else
                    {
                        return newUri.AbsolutePath + newUri.Fragment;
                    }
                }
                
                if (RedirectNodeId.HasValue)
                {
                    return _umbracoHelper.GetUrl(RedirectNodeId.Value);
                    //calculatedRedirectUri = new Uri();
                }
                return string.Empty;

            }
        }
        [JsonIgnore]
        public IPublishedContent RedirectRootNode
        {
            get
            {
                IPublishedContent redirectRootNode = _umbracoHelper.Content(RedirectRootNodeId);
                if (redirectRootNode.Id == 0)
                {
                    var rootNode = _umbracoHelper.Content(-1).Children.FirstOrDefault();
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
                if (UrlTrackerSettings.HasDomainOnChildNode)
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
        public bool RedirectNodeIsPublished
        {
            get
            {
                if (RedirectNodeId.HasValue)
                {
                    return _umbracoHelper.Content(RedirectNodeId.Value).IsPublished();
                }
                return true;
            }
        }
        #endregion

        public UrlTrackerModel() {
            _umbracoHelper = (UmbracoHelper)Current.Factory.GetInstance(typeof(UmbracoHelper));
        }

        public UrlTrackerModel(string oldUrl, string oldUrlQueryString, string oldRegex, int redirectRootNodeId, int? redirectNodeId, string redirectUrl, int redirectHttpCode, bool redirectPassThroughQueryString, bool forceRedirect, string notes)
        {
            OldUrl = oldUrl;
            OldUrlQueryString = oldUrlQueryString;
            OldRegex = oldRegex;
            RedirectRootNodeId = redirectRootNodeId;
            RedirectNodeId = redirectNodeId;
            RedirectUrl = redirectUrl;
            RedirectHttpCode = redirectHttpCode;
            RedirectPassThroughQueryString = redirectPassThroughQueryString;
            ForceRedirect = forceRedirect;
            Notes = notes ?? string.Empty;
            _umbracoHelper = (UmbracoHelper)Current.Factory.GetInstance(typeof(UmbracoHelper));
        }

        public UrlTrackerModel(int id, string oldUrl, string oldUrlQueryString, string oldRegex, int redirectRootNodeId, int? redirectNodeId, string redirectUrl, int redirectHttpCode, bool redirectPassThroughQueryString, bool forceRedirect, string notes, bool is404, string referrer, DateTime inserted)
            : this(oldUrl, oldUrlQueryString, oldRegex, redirectRootNodeId, redirectNodeId, redirectUrl, redirectHttpCode, redirectPassThroughQueryString, forceRedirect, notes)
        {
            Id = id;
            Is404 = is404;
            Referrer = referrer;
            Inserted = inserted;
        }
    }
}