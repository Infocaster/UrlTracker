using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using UrlTracker.Core;
using UrlTracker.Core.Abstractions;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Models;

namespace UrlTracker.Web.Processing
{
    public class ContentRedirectConverter : IRedirectToUrlConverter
    {
        private readonly IUmbracoContextFactoryAbstraction _umbracoContextFactoryAbstraction;
        private readonly IOptionsMonitor<RequestHandlerSettings> _requestHandlerSettings;

        public ContentRedirectConverter(IUmbracoContextFactoryAbstraction umbracoContextFactoryAbstraction, IOptionsMonitor<RequestHandlerSettings> requestHandlerSettings)
        {
            _umbracoContextFactoryAbstraction = umbracoContextFactoryAbstraction;
            _requestHandlerSettings = requestHandlerSettings;
        }

        public bool CanHandle(Redirect redirect)
            => redirect.Target is ContentPageTargetStrategy;

        public string? Handle(Redirect redirect, HttpContext context)
        {
            var target = (ContentPageTargetStrategy)redirect.Target;

            // content might no longer exist, therefore this redirect can no longer be applied
            if (target.Content is null) return null;

            var url = Url.Parse(target.Content.Url(_umbracoContextFactoryAbstraction, culture: target.Culture.DefaultIfNullOrWhiteSpace(null), UrlMode.Absolute));

            if (url.Host != context.Request.Host.Host)
            {
                // convert to better url?
            }

            var requestHandlerSettingsValue = _requestHandlerSettings.CurrentValue;
            return url.ToString(UrlType.Absolute, requestHandlerSettingsValue.AddTrailingSlash);
        }
    }
}
