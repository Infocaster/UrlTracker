using System.Collections.Generic;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace UrlTracker.Web.Events
{
    public interface IContentValueReaderFactory
    {
        IReadOnlyCollection<IContentValueReader> Create(IContent content, bool onlyChanged = false);
    }
}