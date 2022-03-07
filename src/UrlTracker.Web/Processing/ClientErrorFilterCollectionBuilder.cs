using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core.Composing;

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
        public async ValueTask<bool> EvaluateCandidacyAsync(HttpContext httpContext)
        {
            foreach (var filter in this)
            {
                if (!await filter.EvaluateCandidateAsync(httpContext)) return false;
            }

            return true;
        }
    }
}
