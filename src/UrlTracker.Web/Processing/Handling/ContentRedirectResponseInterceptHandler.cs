using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using UrlTracker.Core;
using UrlTracker.Core.Abstractions;
using UrlTracker.Core.Models;
using UrlTracker.Web.Abstraction;

namespace UrlTracker.Web.Processing.Handling
{
    public class ContentRedirectResponseInterceptHandler
        : RedirectResponseInterceptHandler<ContentPageTargetStrategy>
    {
        private readonly IOptionsMonitor<RequestHandlerSettings> _requestHandlerSettings;

        public ContentRedirectResponseInterceptHandler(ILogger<ContentRedirectResponseInterceptHandler> logger,
                                                       IResponseAbstraction responseAbstraction,
                                                       IUmbracoContextFactoryAbstraction umbracoContextFactory,
                                                       IOptionsMonitor<RequestHandlerSettings> requestHandlerSettings)
            : base(logger, responseAbstraction, umbracoContextFactory)
        {
            _requestHandlerSettings = requestHandlerSettings;
        }

        protected override string? GetUrl(HttpContext context, Redirect intercept, ContentPageTargetStrategy target)
        {
            // content might no longer exist, therefore this redirect can no longer be applied
            if (target.Content is null) return null;

            var url = Url.Parse(target.Content.Url(UmbracoContextFactory, culture: target.Culture.DefaultIfNullOrWhiteSpace(null), UrlMode.Absolute));

            if (intercept.RetainQuery) url.Query = context.Request.QueryString.Value;

            var requestHandlerSettingsValue = _requestHandlerSettings.CurrentValue;
            return url.ToString(UrlType.Absolute, requestHandlerSettingsValue.AddTrailingSlash);
        }
    }
}
