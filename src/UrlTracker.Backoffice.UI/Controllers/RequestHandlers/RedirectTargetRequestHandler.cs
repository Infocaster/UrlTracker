using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;
using UrlTracker.Backoffice.UI.Controllers.Models.RedirectTarget;

namespace UrlTracker.Backoffice.UI.Controllers.RequestHandlers
{
    internal interface IRedirectTargetRequestHandler
    {
        ContentTargetResponse? GetContentTarget(GetContentTargetRequest request);
    }

    internal class RedirectTargetRequestHandler : IRedirectTargetRequestHandler
    {
        private readonly IContentService _contentService;
        private readonly IUmbracoContextFactory _umbracoContextFactory;

        public RedirectTargetRequestHandler(IContentService contentService, IUmbracoContextFactory umbracoContextFactory)
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

        private string? TryGetUrl(int contentId)
        {
            using var cref = _umbracoContextFactory.EnsureUmbracoContext();
            var publishedContent = cref.UmbracoContext.Content!.GetById(contentId);

            if (publishedContent is null) return null;

            return publishedContent.Url();
        }
    }
}
