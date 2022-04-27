using Umbraco.Core.Models.PublishedContent;

namespace UrlTracker.Web.Components
{
    public interface IContentValueReader
    {
        string GetCulture();
        string GetName();
        string GetName(IPublishedContent content);
        string GetValue(string propertyValue);
        string GetValue(IPublishedContent content, string propertyAlias);
    }
}