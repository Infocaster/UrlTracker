using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace UrlTracker.Backoffice.UI.Controllers.Models.Base
{
    [DataContract]
    internal class CollectionResponseBase<T>
        where T : notnull
    {
        public CollectionResponseBase(IEnumerable<T> results)
        {
            Results = results.ToList();
        }

        [DataMember(Name = "results")]
        public IReadOnlyCollection<T> Results { get; set; }
    }
}
