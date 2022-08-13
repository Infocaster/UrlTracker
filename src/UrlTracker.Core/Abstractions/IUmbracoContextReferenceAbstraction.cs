using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace UrlTracker.Core.Abstractions
{
    public interface IUmbracoContextReferenceAbstraction
        : IDisposable
    {
        IPublishedContent? GetContentById(int id);
        string GetMediaUrl(IPublishedContent content, UrlMode mode, string? culture);
        string GetUrl(IPublishedContent content, UrlMode mode, string? culture);
        int? GetResponseCode();
        IEnumerable<IPublishedContent> GetContentAtRoot();
    }
}