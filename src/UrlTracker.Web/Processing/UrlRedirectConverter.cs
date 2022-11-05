using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Models;

namespace UrlTracker.Web.Processing
{
    public class UrlRedirectConverter : IRedirectToUrlConverter
    {
        private readonly IOptionsMonitor<RequestHandlerSettings> _requestHandlerSettings;

        public UrlRedirectConverter(IOptionsMonitor<RequestHandlerSettings> requestHandlerSettings)
        {
            _requestHandlerSettings = requestHandlerSettings;
        }

        public bool CanHandle(Redirect redirect)
            => redirect.Target is UrlTargetStrategy;

        public string? Handle(Redirect redirect, HttpContext context)
        {
            var target = (UrlTargetStrategy)redirect.Target;
            var url = target.Url;
            var request = context.Request;

            if (string.IsNullOrEmpty(url.Host)) url.Host = request.Host.Host;

            if (!url.Protocol.HasValue) url.Protocol = (Protocol)Enum.Parse(typeof(Protocol), request.Scheme, true);

            if (redirect.RetainQuery) url.Query = request.QueryString.Value;

            var requestHandlerSettingsValue = _requestHandlerSettings.CurrentValue;
            var result = url.ToString(UrlType.Absolute, requestHandlerSettingsValue.AddTrailingSlash);

            // If redirect is a regex match, ensure that potential capture tokens are replaced in the target url
            // TODO: Evaluate side effects!
            //    This logic has been taken from the old code base. It has a potential side effect.
            //    If a pattern matches on a partial string, the non-matched part will stay in the url
            //    example: regex:"(ipsum)" targeturl: "http://example.com/$1" input: "lorem/ipsum/dolor" result: "lorem/http://example.com/ipsum/dolor"
            if (redirect.Source is RegexSourceStrategy regexsource)
            {
                result = Regex.Replace((context.Request.Path + context.Request.QueryString.Value).TrimStart('/'), regexsource.Value, result);
            }

            return result;
        }
    }
}
