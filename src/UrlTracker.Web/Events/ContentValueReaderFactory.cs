using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Core.Models;
using Umbraco.Extensions;

namespace UrlTracker.Web.Events
{
    [ExcludeFromCodeCoverage]
    public class ContentValueReaderFactory
        : IContentValueReaderFactory
    {
        public IReadOnlyCollection<IContentValueReader> Create(IContent content, bool onlyChanged = false)
        {
            var result = new List<IContentValueReader>();

            if (content.ContentType.VariesByCulture())
            {
                foreach (var culture in content.CultureInfos!)
                {
                    if (!onlyChanged || culture.IsDirty())
                    {
                        result.Add(new ContentWithCultureValueReader(content, culture));
                    }
                }
            }
            else
            {
                result.Add(new ContentWithCultureValueReader(content, null));
            }

            return result;
        }
    }
}
