using Umbraco.Cms.Core.Models.PublishedContent;

namespace UrlTracker.Backoffice.Notifications.Content
{
    /// <summary>
    /// When implemented, this type provides helper methods to read content values from content objects
    /// </summary>
    public interface IContentValueReader
    {
        /// <summary>
        /// When implemented, this method returns the relevant culture for this reader
        /// </summary>
        /// <returns>A <see langword="string" /> that represents a culture</returns>
        string? GetCulture();

        /// <summary>
        /// When implemented, this method returns the relevant name for this reader
        /// </summary>
        /// <returns>A <see langword="string" /> that represents the name of a content item</returns>
        string? GetName();

        /// <summary>
        /// When implemented, this method returns the relevant name of a given content relevant for this reader
        /// </summary>
        /// <param name="content">The content from which the name should be read</param>
        /// <returns>A <see langword="string" /> that represents the name of <paramref name="content"/></returns>
        string? GetName(IPublishedContent content);

        /// <summary>
        /// When implemented, this method returns the relevant property value for this reader
        /// </summary>
        /// <param name="propertyValue">The alias of the property</param>
        /// <returns>A <see langword="string" /> that represents the value of the given property</returns>
        string? GetValue(string propertyValue);

        /// <summary>
        /// When implemented, this method returns the relevant property value of a given content relevant for this reader
        /// </summary>
        /// <param name="content">The content from which the property value should be read</param>
        /// <param name="propertyAlias">The alias of the property</param>
        /// <returns>A <see langword="string" /> that represents the value of the given property</returns>
        string? GetValue(IPublishedContent content, string propertyAlias);
    }
}