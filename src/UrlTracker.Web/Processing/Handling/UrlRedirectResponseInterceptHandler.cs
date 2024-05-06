using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;
using UrlTracker.Core.Abstractions;
using UrlTracker.Core.Logging;
using UrlTracker.Core.Models;
using UrlTracker.Web.Abstraction;

namespace UrlTracker.Web.Processing.Handling
{
    public class UrlRedirectResponseInterceptHandler : RedirectResponseInterceptHandler<UrlTargetStrategy>
    {
        private readonly IOptionsMonitor<RequestHandlerSettings> _requestHandlerSettings;

        public UrlRedirectResponseInterceptHandler(ILogger<UrlRedirectResponseInterceptHandler> logger,
                                    IResponseAbstraction responseAbstraction,
                                    IUmbracoContextFactoryAbstraction umbracoContextFactory,
                                    IOptionsMonitor<RequestHandlerSettings> requestHandlerSettings)
            : base(logger, responseAbstraction, umbracoContextFactory)
        {
            _requestHandlerSettings = requestHandlerSettings;
        }

        protected override string? GetUrl(HttpContext context, Redirect intercept, UrlTargetStrategy target)
        {
            var urlString = target.Url;
            var request = context.Request;

            // If redirect is a regex match, ensure that potential capture tokens are replaced in the target url
            // TODO: Evaluate side effects!
            //    This logic has been taken from the old code base. It has a potential side effect.
            //    If a pattern matches on a partial string, the non-matched part will stay in the url
            //    example: regex:"(ipsum)" targeturl: "http://example.com/$1" input: "lorem/ipsum/dolor" result: "lorem/http://example.com/ipsum/dolor"
            if (intercept.Source is RegexSourceStrategy regexsource)
            {
                urlString = Regex.Replace((context.Request.Path + context.Request.QueryString.Value).TrimStart('/'), regexsource.Value, urlString);
            }

            var url = Url.Parse(urlString);

            if (string.IsNullOrEmpty(url.Host)) url.Host = request.Host.Host;

            if (!url.Protocol.HasValue) url.Protocol = (Protocol)Enum.Parse(typeof(Protocol), request.Scheme, true);

            if (intercept.RetainQuery) url.Query = request.QueryString.Value;

            var requestHandlerSettingsValue = _requestHandlerSettings.CurrentValue;
            var result = url.ToString(UrlType.Absolute, requestHandlerSettingsValue.AddTrailingSlash);

            return result;
        }
    }
}
