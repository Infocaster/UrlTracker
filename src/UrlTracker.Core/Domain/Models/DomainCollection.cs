using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace UrlTracker.Core.Domain.Models
{
    [ExcludeFromCodeCoverage]
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class DomainCollection
        : IReadOnlyCollection<Domain>
    {
        private readonly IReadOnlyCollection<Domain> _elements;

        private DomainCollection(IReadOnlyCollection<Domain> elements)
        {
            _elements = elements;
        }

        public int Count => _elements.Count;

        public IEnumerator<Domain> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_elements).GetEnumerator();
        }

        public static DomainCollection Create(IEnumerable<Domain> elements)
        {
            if (elements is null)
            {
                throw new ArgumentNullException(nameof(elements));
            }

            return new DomainCollection(elements.ToList());
        }

        private string GetDebuggerDisplay()
        {
            return $"{Count} domains";
        }
    }
}
