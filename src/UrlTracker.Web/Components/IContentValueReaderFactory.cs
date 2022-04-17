using System.Collections.Generic;
using Umbraco.Core.Models;

namespace UrlTracker.Web.Components
{
    public interface IContentValueReaderFactory
    {
        IReadOnlyCollection<IContentValueReader> Create(IContent content, bool onlyChanged = false);
    }
}