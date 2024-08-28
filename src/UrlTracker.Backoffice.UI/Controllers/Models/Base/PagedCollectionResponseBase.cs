using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UrlTracker.Backoffice.UI.Controllers.Models.Base
{
    [DataContract]
    internal class PagedCollectionResponseBase<T>
        : CollectionResponseBase<T>
        where T : notnull
    {
        public PagedCollectionResponseBase(IEnumerable<T> results, long total)
            : base(results)
        {
            Total = total;
        }

        [DataMember(Name = "total")]
        public long Total { get; set; }
    }
}
