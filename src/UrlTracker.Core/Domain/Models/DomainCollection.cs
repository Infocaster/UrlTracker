using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Domain.Models
{
    [ExcludeFromCodeCoverage]
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class DomainCollection : UrlTrackerCollectionBase<DomainCollection, Domain>, IReadOnlyCollection<Domain>
    {
        public int Count => Elements.Count;

        private string GetDebuggerDisplay()
        {
            return $"{Count} domains";
        }
    }
}
