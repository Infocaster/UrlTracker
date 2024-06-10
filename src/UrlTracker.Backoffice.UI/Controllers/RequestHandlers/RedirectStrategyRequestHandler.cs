using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;
using UrlTracker.Backoffice.UI.Controllers.Models.RedirectStrategy;
using UrlTracker.Backoffice.UI.Controllers.Models.RedirectTarget;

namespace UrlTracker.Backoffice.UI.Controllers.RequestHandlers
{
    internal interface IRedirectStrategyRequestHandler
    {
        ContentTargetResponse? GetContentTarget(GetContentTargetRequest request);
        IEnumerable<RedirectStrategyResponse> GetSources();
        IEnumerable<RedirectStrategyResponse> GetTargets();
    }

    internal class RedirectStrategyRequestHandler : IRedirectStrategyRequestHandler
    {
        private readonly IContentService _contentService;
        private readonly IUmbracoContextFactory _umbracoContextFactory;

        public RedirectStrategyRequestHandler(IContentService contentService, IUmbracoContextFactory umbracoContextFactory)
        {
            _contentService = contentService;
            _umbracoContextFactory = umbracoContextFactory;
        }

        public ContentTargetResponse? GetContentTarget(GetContentTargetRequest request)
        {
            var content = _contentService.GetById(request.Id!.Value);
            if (content is null || content.Trashed) return null;

            var iconComponents = content.ContentType.Icon!.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var iconColor = iconComponents.Length > 1 ? iconComponents[1] : null;

            var url = TryGetUrl(content.Id);

            return new ContentTargetResponse(iconComponents[0], iconColor, content.GetCultureName(request.Culture) ?? content.Name!, url);
        }

        public IEnumerable<RedirectStrategyResponse> GetSources()
        {
            var result = new List<RedirectStrategyResponse>
            {
                new("Url", Core.Defaults.DatabaseSchema.RedirectSourceStrategies.Url),
                new("Regex", Core.Defaults.DatabaseSchema.RedirectSourceStrategies.RegularExpression),
            };

            return result;
        }

        public IEnumerable<RedirectStrategyResponse> GetTargets()
        {
            var result = new List<RedirectStrategyResponse>
            {
                new("url", Core.Defaults.DatabaseSchema.RedirectTargetStrategies.Url),
                new("content", Core.Defaults.DatabaseSchema.RedirectTargetStrategies.Content),
            };

            return result;
        }

        private string? TryGetUrl(int contentId)
        {
            using var cref = _umbracoContextFactory.EnsureUmbracoContext();
            var publishedContent = cref.UmbracoContext.Content!.GetById(contentId);

            if (publishedContent is null) return null;

            return publishedContent.Url();
        }
    }
}
