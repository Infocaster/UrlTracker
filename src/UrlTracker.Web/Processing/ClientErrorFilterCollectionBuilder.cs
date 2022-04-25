using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Composing;
using UrlTracker.Web.Events.Models;

namespace UrlTracker.Web.Processing
{
    [ExcludeFromCodeCoverage]
    public class ClientErrorFilterCollectionBuilder
        : OrderedCollectionBuilderBase<ClientErrorFilterCollectionBuilder, ClientErrorFilterCollection, IClientErrorFilter>
    {
        protected override ClientErrorFilterCollectionBuilder This => this;
    }

    [ExcludeFromCodeCoverage]
    public class ClientErrorFilterCollection
        : BuilderCollectionBase<IClientErrorFilter>, IClientErrorFilterCollection
    {
        public ClientErrorFilterCollection(Func<IEnumerable<IClientErrorFilter>> items)
            : base(items)
        { }

        // asynchronously evaluates all the filters and returns false if any of them return false
        public async ValueTask<bool> EvaluateCandidacyAsync(UrlTrackerHandled notification)
        {
            foreach (var filter in this)
            {
                if (!await filter.EvaluateCandidateAsync(notification)) return false;
            }

            return true;
        }
    }
}
