using InfoCaster.Umbraco.UrlTracker.Extensions;
using InfoCaster.Umbraco.UrlTracker.Helpers;
using InfoCaster.Umbraco.UrlTracker.Models;
using InfoCaster.Umbraco.UrlTracker.Repositories;
using InfoCaster.Umbraco.UrlTracker.Services;
using InfoCaster.Umbraco.UrlTracker.Settings;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Web;
using Umbraco.Web.Composing;
using Umbraco.Web.Routing;

namespace InfoCaster.Umbraco.UrlTracker.Modules
{
    public class UrlTrackerModule : IHttpModule
    {
        private IUrlTrackerHelper _urlTrackerHelper => DependencyResolver.Current.GetService<IUrlTrackerHelper>();
        private IUrlTrackerService _urlTrackerService => DependencyResolver.Current.GetService<IUrlTrackerService>();
        private IUrlTrackerSettings _urlTrackerSettings => DependencyResolver.Current.GetService<IUrlTrackerSettings>();
        private IUrlTrackerRepository _urlTrackerRepository => DependencyResolver.Current.GetService<IUrlTrackerRepository>();
        private IUrlTrackerLoggingHelper _urlTrackerLoggingHelper => DependencyResolver.Current.GetService<IUrlTrackerLoggingHelper>();

        static Regex _capturingGroupsRegex = new Regex("\\$\\d+");
        static bool _urlTrackerInstalled;
        private static bool _urlTrackerSubscribed = false;
        private static readonly object _urlTrackerSubscribeLock = new object();

        public UrlTrackerModule() { }

        public static event EventHandler<HttpResponse> PreUrlTracker;

        #region IHttpModule Members
        public void Dispose() { }

        public void Init(HttpApplication app)
        {
            if (_urlTrackerLoggingHelper != null)
            {
                _urlTrackerInstalled = true;

                if (!_urlTrackerSubscribed)
                {
                    lock (_urlTrackerSubscribeLock)
                    {
                        UmbracoModule.EndRequest += UmbracoModule_EndRequest;

                        _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | Subscribed to EndRequest events");
                        _urlTrackerSubscribed = true;
                    }
                }

                // Prevent YSOD crash
                // https://stackoverflow.com/questions/3712598/httpmodule-init-safely-add-httpapplication-beginrequest-handler-in-iis7-integr
                app.PostResolveRequestCache -= Context_PostResolveRequestCache;
                app.PostResolveRequestCache += Context_PostResolveRequestCache;

                _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | Subscribed to AcquireRequestState events");
            }
        }

        void Context_PostResolveRequestCache(object sender, EventArgs e)
        {
            UrlTrackerDo("AcquireRequestState", ignoreHttpStatusCode: true, context: HttpContext.Current);
        }

        void UmbracoModule_EndRequest(object sender, UmbracoRequestEventArgs args)
        {
            UrlTrackerDo("EndRequest", context: args.HttpContext.ApplicationInstance.Context);
        }
        #endregion

        void UrlTrackerDo(string callingEventName, bool ignoreHttpStatusCode = false, HttpContext context = null)
        {
            if (_urlTrackerSettings.IsDisabled())
            {
                _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | UrlTracker is disabled by config");
                return;
            }

            if (context == null)
            {
                _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | No HttpContext has been passed by {0}", callingEventName);
                context = HttpContext.Current;
            }

            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            if (!string.IsNullOrEmpty(request.QueryString[_urlTrackerSettings.GetHttpModuleCheck()]))
            {
                response.Clear();
                response.Write(_urlTrackerSettings.GetHttpModuleCheck());
                response.StatusCode = 200;
                response.End();
                return;
            }

            _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | {0} start", callingEventName);

            _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | Incoming URL is: {0}", _urlTrackerHelper.ResolveShortestUrl(request.RawUrl));

            if (_urlTrackerInstalled && (response.StatusCode == (int)HttpStatusCode.NotFound || ignoreHttpStatusCode))
            {
                if (response.StatusCode == (int)HttpStatusCode.NotFound)
                    _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | Response statusCode is 404, continue URL matching");
                else
                    _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | Checking for forced redirects (AcquireRequestState), continue URL matching");

                var urlWithoutDomain = "";
                var domain = _urlTrackerService.GetUmbracoDomainFromUrl(request.Url.ToString(), ref urlWithoutDomain);
                var shortestUrl = _urlTrackerHelper.ResolveShortestUrl(urlWithoutDomain);

                int rootNodeId = -1;
                var urlHasQueryString = shortestUrl.Contains('?');
                var urlWithoutQueryString = (urlHasQueryString ? shortestUrl.Substring(0, shortestUrl.IndexOf('?')) : shortestUrl);

                if (_urlTrackerHelper.IsReservedPathOrUrl(shortestUrl))
                {
                    _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | URL is an umbraco reserved path or url, ignore request");
                    return;
                }

                if (domain != null)
                    rootNodeId = domain.NodeId;
                else if (Current.UmbracoContext != null)
                    rootNodeId = Current.UmbracoContext.Content.GetAtRoot().FirstOrDefault()?.Root()?.Id ?? -1;

                _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | Current request's rootNodeId: {0}", rootNodeId);

                string redirectUrl = null;
                int? redirectHttpCode = null;
                bool redirectPassThroughQueryString = true;

                if (!ignoreHttpStatusCode) // Normal matching (database)
                    LoadUrlTrackerMatchesFromDatabase(request, domain, shortestUrl, urlWithoutQueryString, urlHasQueryString, rootNodeId, ref redirectUrl, ref redirectHttpCode, ref redirectPassThroughQueryString);
                else // Forced matching (cache)
                    LoadUrlTrackerMatchesFromCache(request, domain, shortestUrl, urlWithoutQueryString, urlHasQueryString, rootNodeId, ref redirectUrl, ref redirectHttpCode, ref redirectPassThroughQueryString);

                if (!redirectHttpCode.HasValue)
                {
                    if (!ignoreHttpStatusCode)
                    {
                        // Normal matching (database)
                        // Regex matching

                        var query = "SELECT * FROM icUrlTracker WHERE Is404 = 0 AND ForceRedirect = @forceRedirect AND (RedirectRootNodeId = @redirectRootNodeId OR RedirectRootNodeId = -1) AND OldRegex IS NOT NULL ORDER BY Inserted DESC";
                        UrlTrackerModel result = _urlTrackerRepository.FirstOrDefault<UrlTrackerModel>(query,
                            new
                            {
                                forceRedirect = ignoreHttpStatusCode ? 1 : 0,
                                redirectRootNodeId = rootNodeId
                            });

                        if (result != null)
                        {
                            var regex = new Regex(result.OldRegex);

                            if (regex.IsMatch(shortestUrl))
                            {
                                _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | Regex match found");

                                if (result.RedirectNodeId.HasValue)
                                {
                                    int redirectNodeId = result.RedirectNodeId.Value;
                                    _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | Redirect node id: {0}", redirectNodeId);

                                    var rootNode = _urlTrackerService.GetNodeById(rootNodeId);
                                    if (rootNode != null && rootNode.Name != null && rootNode.Id > 0)
                                    {
                                        redirectUrl = _urlTrackerService.GetUrlByNodeId(redirectNodeId);
                                        _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | Redirect url set to: {0}", redirectUrl);
                                    }
                                    else
                                        _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | Redirect node is invalid; node is null, name is null or id <= 0");
                                }
                                else if (!string.IsNullOrWhiteSpace(result.RedirectUrl))
                                {
                                    redirectUrl = result.RedirectUrl;
                                    _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | Redirect url set to: {0}", redirectUrl);

                                    if (_capturingGroupsRegex.IsMatch(redirectUrl))
                                    {
                                        _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | Found regex capturing groups in the redirect url");
                                        redirectUrl = regex.Replace(shortestUrl, redirectUrl);

                                        _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | Redirect url changed to: {0} (because of regex capturing groups)", redirectUrl);
                                    }
                                }

                                redirectPassThroughQueryString = result.RedirectPassThroughQueryString;
                                _urlTrackerLoggingHelper.LogInformation(
                                    "UrlTracker HttpModule | PassThroughQueryString is enabled");

                                redirectHttpCode = result.RedirectHttpCode;
                                _urlTrackerLoggingHelper.LogInformation(
                                    "UrlTracker HttpModule | Redirect http code set to: {0}", redirectHttpCode);
                            }
                        }
                    }
                    else
                    {
                        // Forced matching (cache)
                        List<UrlTrackerModel> forcedRedirects = _urlTrackerService.GetForcedRedirects().Where(x => !string.IsNullOrEmpty(x.OldRegex)).ToList();

                        if (forcedRedirects == null || !forcedRedirects.Any())
                            return;

                        foreach (var match in forcedRedirects
                            .Where(x =>
                                (x.Culture == (domain?.LanguageIsoCode?.ToLower() ?? "") || x.Culture == null) &&
                                (x.RedirectRootNodeId == -1 || x.RedirectRootNodeId == rootNodeId))
                            .Select(x => new { UrlTrackerModel = x, Regex = new Regex(x.OldRegex) })
                            .Where(x => x.Regex.IsMatch(shortestUrl)))
                        {
                            _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | Regex match found");

                            if (match.UrlTrackerModel.RedirectNodeId.HasValue)
                            {
                                _urlTrackerLoggingHelper.LogInformation(
                                    "UrlTracker HttpModule | Redirect node id: {0}",
                                    match.UrlTrackerModel.RedirectNodeId.Value);

                                var rootNode = _urlTrackerService.GetNodeById(rootNodeId);

                                if (rootNode != null && rootNode.Name != null && rootNode.Id > 0)
                                {
                                    redirectUrl =
                                        _urlTrackerService.GetUrlByNodeId(
                                            match.UrlTrackerModel.RedirectNodeId.Value);
                                    _urlTrackerLoggingHelper.LogInformation(
                                        "UrlTracker HttpModule | Redirect url set to: {0}", redirectUrl);
                                }
                                else
                                    _urlTrackerLoggingHelper.LogInformation(
                                        "UrlTracker HttpModule | Redirect node is invalid; node is null, name is null or id <= 0");
                            }
                            else if (!string.IsNullOrEmpty(match.UrlTrackerModel.RedirectUrl))
                            {
                                redirectUrl = match.UrlTrackerModel.RedirectUrl;
                                _urlTrackerLoggingHelper.LogInformation(
                                    "UrlTracker HttpModule | Redirect url set to: {0}", redirectUrl);

                                if (_capturingGroupsRegex.IsMatch(redirectUrl))
                                {
                                    _urlTrackerLoggingHelper.LogInformation(
                                        "UrlTracker HttpModule | Found regex capturing groups in the redirect url");
                                    redirectUrl = match.Regex.Replace(shortestUrl, redirectUrl);

                                    _urlTrackerLoggingHelper.LogInformation(
                                        "UrlTracker HttpModule | Redirect url changed to: {0} (because of regex capturing groups)",
                                        redirectUrl);
                                }
                            }

                            redirectPassThroughQueryString = match.UrlTrackerModel.RedirectPassThroughQueryString;
                            _urlTrackerLoggingHelper.LogInformation(
                                "UrlTracker HttpModule | PassThroughQueryString is enabled");

                            redirectHttpCode = match.UrlTrackerModel.RedirectHttpCode;
                            _urlTrackerLoggingHelper.LogInformation(
                                "UrlTracker HttpModule | Redirect http code set to: {0}", redirectHttpCode);
                        }
                    }
                }

                if (redirectHttpCode.HasValue)
                {
                    string redirectLocation = string.Empty;

                    if (!string.IsNullOrEmpty(redirectUrl))
                    {
                        if (redirectUrl == "/")
                            redirectUrl = string.Empty;

                        var redirectUri = new Uri(
                            redirectUrl.StartsWith(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
                                ? redirectUrl
                                : string.Format("{0}{1}{2}{3}/{4}", request.Url.Scheme, Uri.SchemeDelimiter,
                                    request.Url.Host,
                                    request.Url.Port != 80 && _urlTrackerSettings.AppendPortNumber()
                                        ? string.Concat(":", request.Url.Port)
                                        : string.Empty,
                                    redirectUrl.StartsWith("/") ? redirectUrl.Substring(1) : redirectUrl));

                        if (redirectPassThroughQueryString)
                        {
                            NameValueCollection redirectQueryString = HttpUtility.ParseQueryString(redirectUri.Query);
                            NameValueCollection newQueryString = HttpUtility.ParseQueryString(request.Url.Query);

                            if (redirectQueryString.HasKeys())
                                newQueryString = newQueryString.Merge(redirectQueryString);

                            string pathAndQuery = Uri.UnescapeDataString(redirectUri.PathAndQuery) + redirectUri.Fragment;

                            redirectUri = new Uri(string.Format("{0}{1}{2}{3}/{4}{5}",
                                redirectUri.Scheme,
                                Uri.SchemeDelimiter,
                                redirectUri.Host,
                                redirectUri.Port != 80 && _urlTrackerSettings.AppendPortNumber()
                                    ? string.Concat(":", redirectUri.Port)
                                    : string.Empty,
                                pathAndQuery.Contains('?') ? pathAndQuery.Substring(0, pathAndQuery.IndexOf('?')) :
                                pathAndQuery.StartsWith("/") ? pathAndQuery.Substring(1) : pathAndQuery,
                                newQueryString.HasKeys()
                                    ? string.Concat("?", newQueryString.ToQueryString())
                                    : string.Empty));
                        }

                        if (redirectUri == new Uri(string.Format("{0}{1}{2}{3}/{4}", request.Url.Scheme,
                            Uri.SchemeDelimiter, request.Url.Host,
                            request.Url.Port != 80 && _urlTrackerSettings.AppendPortNumber()
                                ? string.Concat(":", request.Url.Port)
                                : string.Empty,
                            request.RawUrl.StartsWith("/") ? request.RawUrl.Substring(1) : request.RawUrl)))
                        {
                            _urlTrackerLoggingHelper.LogInformation(
                                "UrlTracker HttpModule | Redirect URL is the same as Request.RawUrl; don't redirect");
                            return;
                        }

                        if (request.Url.Host.Equals(redirectUri.Host, StringComparison.OrdinalIgnoreCase))
                            redirectLocation = redirectUri.PathAndQuery + redirectUri.Fragment;
                        else
                            redirectLocation = redirectUri.AbsoluteUri;

                        _urlTrackerLoggingHelper.LogInformation(
                            "UrlTracker HttpModule | Response redirectlocation set to: {0}", redirectLocation);
                    }

                    response.Clear();
                    response.StatusCode = redirectHttpCode.Value;

                    _urlTrackerLoggingHelper.LogInformation(
                        "UrlTracker HttpModule | Response statuscode set to: {0}", response.StatusCode);

                    if (PreUrlTracker != null)
                    {
                        PreUrlTracker(null, response);
                        _urlTrackerLoggingHelper.LogInformation(
                            "UrlTracker HttpModule | Custom event has been called: {0}", PreUrlTracker.Method.Name);
                    }

                    if (!string.IsNullOrEmpty(redirectLocation))
                        response.RedirectLocation = redirectLocation;

                    response.End();
                }
                else if (!ignoreHttpStatusCode)
                {
                    var ignoreExist = _urlTrackerService.IgnoreExist(shortestUrl, rootNodeId, domain?.LanguageIsoCode);

                    // Log 404
                    if (!ignoreExist &&
                        !_urlTrackerSettings.IsNotFoundTrackingDisabled() &&
                        !_urlTrackerHelper.IsReservedPathOrUrl(urlWithoutQueryString) &&
                        request.Headers["X-UrlTracker-Ignore404"] != "1")
                    {
                        bool ignoreNotFoundBasedOnRegexPatterns = false;

                        foreach (Regex regexNotFoundUrlToIgnore in _urlTrackerSettings.GetRegexNotFoundUrlsToIgnore())
                        {
                            if (regexNotFoundUrlToIgnore.IsMatch(urlWithoutQueryString))
                            {
                                ignoreNotFoundBasedOnRegexPatterns = true;
                                break;
                            }
                        }

                        if (!ignoreNotFoundBasedOnRegexPatterns)
                        {
                            _urlTrackerLoggingHelper.LogInformation(
                                "UrlTracker HttpModule | No match found, logging as 404 not found");

                            _urlTrackerService.AddNotFound(
                                shortestUrl,
                                rootNodeId,
                                (request.UrlReferrer != null && !request.UrlReferrer.ToString()
                                    .Contains(_urlTrackerSettings.GetReferrerToIgnore())
                                    ? request.UrlReferrer.ToString()
                                    : ""),
                                domain?.LanguageIsoCode
                            );
                        }
                    }

                    if (_urlTrackerSettings.IsNotFoundTrackingDisabled())
                        _urlTrackerLoggingHelper.LogInformation(
                            "UrlTracker HttpModule | No match found and not found (404) tracking is disabled");
                    else if (ignoreExist)
                        _urlTrackerLoggingHelper.LogInformation(
                            "UrlTracker HttpModule | No match found, url is configured to be ignored: {0}",
                            urlWithoutQueryString);
                    else if (_urlTrackerHelper.IsReservedPathOrUrl(urlWithoutQueryString))
                        _urlTrackerLoggingHelper.LogInformation(
                            "UrlTracker HttpModule | No match found, url is ignored because it's an umbraco reserved URL or path: {0}",
                            urlWithoutQueryString);
                    else if (request.Headers["X-UrlTracker-Ignore404"] == "1")
                        _urlTrackerLoggingHelper.LogInformation(
                            "UrlTracker HttpModule | No match found, url is ignored because the 'X-UrlTracker-Ignore404' header was set to '1'. URL: {0}",
                            urlWithoutQueryString);
                }
                else
                    _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | No match found in {0}",
                        callingEventName);
            }
            else
                _urlTrackerLoggingHelper.LogInformation(
                    "UrlTracker HttpModule | Response statuscode is not 404, UrlTracker won't do anything");

            _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | {0} end", callingEventName);
        }

        void LoadUrlTrackerMatchesFromDatabase(HttpRequest request, UrlTrackerDomain domain, string shortestUrl, string urlWithoutQueryString, bool urlHasQueryString, int rootNodeId, ref string redirectUrl, ref int? redirectHttpCode, ref bool redirectPassThroughQueryString)
        {
            var result = _urlTrackerRepository.FirstOrDefault<UrlTrackerModel>(
                "SELECT * FROM icUrlTracker WHERE Is404 = 0 AND ForceRedirect = 0 AND (Culture = @culture OR Culture IS NULL) AND (RedirectRootNodeId = @redirectRootNodeId OR RedirectRootNodeId IS NULL OR RedirectRootNodeId = -1) AND (OldUrl = @urlWithoutQueryString OR OldUrl = @urlWithQueryString) ORDER BY CASE WHEN RedirectHttpCode = 410 THEN 2 ELSE 1 END",
                new
                {
                    redirectRootNodeId = rootNodeId,
                    urlWithoutQueryString = urlWithoutQueryString,
                    urlWithQueryString = shortestUrl,
                    culture = domain?.LanguageIsoCode?.ToLower() ?? ""
                });

            if (result == null)
                return;

            _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | URL match found");

            if (result.RedirectNodeId.HasValue && result.RedirectHttpCode != (int)HttpStatusCode.Gone)
            {
                var rootNode = result.RedirectRootNode;
                var redirectNodeId = result.RedirectNodeId.Value;
                var culture = result.Culture ?? "";

                _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | Redirect node id: {0}", redirectNodeId);

                if (rootNode != null && rootNode.Id > 0)
                {
                    var tempUrl = _urlTrackerService.GetUrlByNodeId(redirectNodeId, culture);
                    redirectUrl = tempUrl.StartsWith(Uri.UriSchemeHttp) ? tempUrl : string.Format("{0}{1}{2}{3}{4}", HttpContext.Current.Request.Url.Scheme, Uri.SchemeDelimiter, HttpContext.Current.Request.Url.Host, HttpContext.Current.Request.Url.Port != 80 && _urlTrackerSettings.AppendPortNumber() ? string.Concat(":", HttpContext.Current.Request.Url.Port) : string.Empty, tempUrl);

                    if (redirectUrl.StartsWith(Uri.UriSchemeHttp))
                    {
                        Uri redirectUri = new Uri(redirectUrl);
                        string pathAndQuery = Uri.UnescapeDataString(redirectUri.PathAndQuery) + redirectUri.Fragment;

                        redirectUrl = GetCorrectedUrl(redirectUri, rootNodeId, pathAndQuery);
                    }

                    _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | Redirect url set to: {0}", redirectUrl);
                }
                else
                {
                    _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | Redirect node is invalid; node is null, name is null or id <= 0");
                    return;
                }
            }
            else if (!string.IsNullOrWhiteSpace(result.RedirectUrl))
            {
                redirectUrl = result.RedirectUrl;
                _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | Redirect url set to: {0}", redirectUrl);
            }

            redirectPassThroughQueryString = result.RedirectPassThroughQueryString;
            _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | PassThroughQueryString is {0}", redirectPassThroughQueryString ? "enabled" : "disabled");

            NameValueCollection OldUrlQuery = null;
            if (!string.IsNullOrWhiteSpace(result.OldUrlQuery))
            {
                OldUrlQuery = HttpUtility.ParseQueryString(result.OldUrlQuery);
                _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | Old URL query string set to: {0}", OldUrlQuery.ToQueryString());
            }

            if ((urlHasQueryString || OldUrlQuery != null) && (OldUrlQuery != null && !request.QueryString.CollectionEquals(OldUrlQuery)))
            {
                _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | Aborting; query strings don't match");
                return;
            }

            redirectHttpCode = result.RedirectHttpCode;
            _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | Redirect http code set to: {0}", redirectHttpCode);
        }

        void LoadUrlTrackerMatchesFromCache(HttpRequest request, UrlTrackerDomain domain, string shortestUrl, string urlWithoutQueryString, bool urlHasQueryString, int rootNodeId, ref string redirectUrl, ref int? redirectHttpCode, ref bool redirectPassThroughQueryString)
        {
            var forcedRedirects = _urlTrackerService.GetForcedRedirects();

            if (forcedRedirects == null || !forcedRedirects.Any())
                return;

            var redirects = forcedRedirects.Where(x =>
                    (x.Culture == (domain?.LanguageIsoCode?.ToLower() ?? "") || x.Culture == null) &&
                    (x.RedirectRootNodeId == rootNodeId || x.RedirectRootNodeId == -1) &&
                    (string.Equals(x.OldUrl ?? "", urlWithoutQueryString ?? "", StringComparison.CurrentCultureIgnoreCase) || string.Equals(x.OldUrl ?? "", shortestUrl ?? "", StringComparison.CurrentCultureIgnoreCase))
                )
                .OrderBy(x => x.RedirectHttpCode == 410 ? 2 : 1)
                .ThenByDescending(x => x.OldUrlQuery).ToList();

            foreach (var forcedRedirect in redirects)
            {
                _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | URL match found");

                if (forcedRedirect.RedirectNodeId.HasValue && forcedRedirect.RedirectHttpCode != (int)HttpStatusCode.Gone)
                {
                    _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | Redirect node id: {0}", forcedRedirect.RedirectNodeId.Value);

                    if (forcedRedirect.RedirectNode == null || string.IsNullOrEmpty(forcedRedirect.RedirectNode.Name) || forcedRedirect.RedirectNode.Id <= 0)
                    {
                        _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | Redirect node is invalid; node is null, name is null or id <= 0");
                        continue;
                    }

                    var culture = forcedRedirect.Culture ?? "";

                    string tempUrl = _urlTrackerService.GetUrlByNodeId(forcedRedirect.RedirectNodeId.Value, culture);
                    redirectUrl = tempUrl.StartsWith(Uri.UriSchemeHttp) ? tempUrl : string.Format("{0}{1}{2}{3}{4}", HttpContext.Current.Request.Url.Scheme, Uri.SchemeDelimiter, HttpContext.Current.Request.Url.Host, HttpContext.Current.Request.Url.Port != 80 && _urlTrackerSettings.AppendPortNumber() ? string.Concat(":", HttpContext.Current.Request.Url.Port) : string.Empty, tempUrl);

                    if (redirectUrl.StartsWith(Uri.UriSchemeHttp))
                    {
                        Uri redirectUri = new Uri(redirectUrl);
                        string pathAndQuery = Uri.UnescapeDataString(redirectUri.PathAndQuery);
                        redirectUrl = GetCorrectedUrl(redirectUri, forcedRedirect.RedirectRootNodeId, pathAndQuery);

                    }
                    _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | Redirect url set to: {0}", redirectUrl);
                }
                else if (!string.IsNullOrEmpty(forcedRedirect.RedirectUrl))
                {
                    redirectUrl = forcedRedirect.RedirectUrl;
                    _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | Redirect url set to: {0}", redirectUrl);
                }

                redirectPassThroughQueryString = forcedRedirect.RedirectPassThroughQueryString;
                _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | PassThroughQueryString is {0}", redirectPassThroughQueryString ? "enabled" : "disabled");

                NameValueCollection OldUrlQuery = null;
                if (!string.IsNullOrEmpty(forcedRedirect.OldUrlQuery))
                {
                    OldUrlQuery = HttpUtility.ParseQueryString(forcedRedirect.OldUrlQuery);
                    _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | Old URL query string set to: {0}", OldUrlQuery.ToQueryString());
                }

                if ((urlHasQueryString || OldUrlQuery != null) && (OldUrlQuery != null && !request.QueryString.CollectionEquals(OldUrlQuery)))
                {
                    _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | Aborting; query strings don't match");
                    continue;
                }

                redirectHttpCode = forcedRedirect.RedirectHttpCode;
                _urlTrackerLoggingHelper.LogInformation("UrlTracker HttpModule | Redirect http code set to: {0}", redirectHttpCode);

                break;
            }
        }

        private string GetCorrectedUrl(Uri redirectUri, int rootNodeId, string pathAndQuery)
        {
            string redirectUrl = pathAndQuery;

            if (redirectUri.Host != HttpContext.Current.Request.Url.Host)
            {
                // if site runs on other domain then current, check if the current domain is already a domain for that site (prevent unnessecary redirect to primary domain)
                var hosts = _urlTrackerService.GetDomains()
                    .Where(x => x.NodeId == rootNodeId)
                    .Select(x => new Uri(x.UrlWithDomain).Host)
                    .ToList();

                if (!hosts.Contains(redirectUri.Host)) // if current domain is not related to target domain, do absoluteUri redirect
                    redirectUrl = new Uri(redirectUri, '/' + pathAndQuery.TrimStart('/')).AbsoluteUri;
            }

            return redirectUrl;
        }
    }
}
