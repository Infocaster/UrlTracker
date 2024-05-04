using System.Collections.Generic;
using Umbraco.Cms.Core.Models;

namespace UrlTracker.Backoffice.Notifications.Content
{
    /// <summary>
    /// When implemented, this type produces instances of <see cref="IContentValueReader"/> to help read relevant values from content
    /// </summary>
    public interface IContentValueReaderFactory
    {
        /// <summary>
        /// When implemented, this method creates and returns a list of <see cref="IContentValueReader"/>s that are relevant for the given <paramref name="content"/>
        /// </summary>
        /// <param name="content">The content for which the reader should be produced</param>
        /// <param name="onlyChanged">Set this value to true to only get readers for content in cultures that have changes</param>
        /// <returns>A list of <see cref="IContentValueReader"/> based on the given <paramref name="content"/></returns>
        IReadOnlyCollection<IContentValueReader> Create(IContent content, bool onlyChanged = false);
    }
}