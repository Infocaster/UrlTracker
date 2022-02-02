using System;
using Umbraco.Core.Models.PublishedContent;

namespace UrlTracker.Core.Abstractions
{
    public interface IUmbracoContextReferenceAbstraction
        : IDisposable
    {
        IPublishedContent GetContentById(int id);
        string GetMediaUrl(IPublishedContent content, UrlMode mode, string culture);
        string GetUrl(IPublishedContent content, UrlMode mode, string culture);
    }
}