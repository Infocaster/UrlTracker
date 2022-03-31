using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace UrlTracker.Core.Models
{
    public class UrlTrackerCollectionBase<TThis, TElement>
        : IEnumerable<TElement>
        where TThis : UrlTrackerCollectionBase<TThis, TElement>, new()
    {
        public UrlTrackerCollectionBase()
        {
            Elements = new List<TElement>();
        }

        public IReadOnlyCollection<TElement> Elements { get; private set; }
        public int Total { get; private set; }

        public static TThis Create(IEnumerable<TElement> elements, int? total = null)
        {
            var elementList = elements.ToList();
            return new TThis()
            {
                Total = total ?? elementList.Count,
                Elements = elementList
            };
        }

        [ExcludeFromCodeCoverage]
        public IEnumerator<TElement> GetEnumerator()
        {
            return Elements.GetEnumerator();
        }

        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Elements).GetEnumerator();
        }
    }
}
