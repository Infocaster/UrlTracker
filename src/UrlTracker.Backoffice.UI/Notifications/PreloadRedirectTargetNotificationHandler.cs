using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;
using UrlTracker.Backoffice.UI.Controllers.Models.Redirects;
using UrlTracker.Backoffice.UI.Controllers.Models.RedirectTarget;

namespace UrlTracker.Backoffice.UI.Notifications;

internal class PreloadRedirectTargetNotificationHandler
    : INotificationHandler<ServingRedirectsNotification>
{
    private readonly IContentService _contentService;
    private readonly IUmbracoContextFactory _umbracoContextFactory;

    public PreloadRedirectTargetNotificationHandler(
        IContentService contentService,
        IUmbracoContextFactory umbracoContextFactory)
    {
        _contentService = contentService;
        _umbracoContextFactory = umbracoContextFactory;
    }

    public void Handle(ServingRedirectsNotification notification)
    {
        var relevantRedirects = notification.Redirects
            .Where(r => r.Target.Strategy == Core.Defaults.DatabaseSchema.RedirectTargetStrategies.Content)
            .ToList();

        if (relevantRedirects.Count == 0) return;

        var contentMap = GetContentMap(relevantRedirects);
        var content = _contentService.GetByIds(contentMap.Select(c => c.ContentId)).ToList();

        foreach (var redirect in relevantRedirects)
        {
            var target = TryGetTarget(redirect, contentMap, content);
            redirect.AdditionalData.Add("content", target);
        }
    }

    private ContentTargetResponse? TryGetTarget(RedirectResponse redirect, List<ContentMapItem> contentMap, List<IContent> content)
    {
        var mapItem = contentMap.FirstOrDefault(c => c.RedirectId == redirect.Id);
        if (mapItem == default) return null;

        var contentItem = content.FirstOrDefault(c => c.Id == mapItem.ContentId);
        if (contentItem is null) return null;

        var iconComponents = contentItem.ContentType.Icon!.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var iconColor = iconComponents.Length > 1 ? iconComponents[1] : null;

        var url = TryGetUrl(contentItem.Id);

        return new ContentTargetResponse(iconComponents[0], iconColor, contentItem.GetCultureName(mapItem.Culture)!, url);
    }

    private string? TryGetUrl(int contentId)
    {
        using var cref = _umbracoContextFactory.EnsureUmbracoContext();
        var publishedContent = cref.UmbracoContext.Content!.GetById(contentId);

        if (publishedContent is null) return null;

        return publishedContent.Url();
    }

    private static List<ContentMapItem> GetContentMap(List<RedirectResponse> relevantRedirects)
    {
        var result = new List<ContentMapItem>(relevantRedirects.Count);
        foreach (var redirect in relevantRedirects)
        {
            var components = redirect.Target.Value.Split(';', StringSplitOptions.RemoveEmptyEntries);
            if (!int.TryParse(components[0], out var id))
            {
                continue;
            }

            string? culture = components.Length > 1 ? components[1] : null;
            result.Add(new ContentMapItem(redirect.Id, id, culture));
        }

        return result;
    }

    private record struct ContentMapItem(int RedirectId, int ContentId, string? Culture);
}
