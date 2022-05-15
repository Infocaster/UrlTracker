using Umbraco.Cms.Core.Models.PublishedContent;

namespace UrlTracker.Web.Events
{
    public interface IContentValueReader
    {
        string? GetCulture();
        string? GetName();
        string? GetName(IPublishedContent content);
        string? GetValue(string propertyValue);
        string? GetValue(IPublishedContent content, string propertyAlias);
    }
}