using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Composing;
using UrlTracker.Core.Models;

namespace UrlTracker.Web.Processing
{
    [ExcludeFromCodeCoverage]
    public class RequestInterceptFilterCollectionBuilder
        : OrderedCollectionBuilderBase<RequestInterceptFilterCollectionBuilder, RequestInterceptFilterCollection, IRequestInterceptFilter>
    {
        protected override RequestInterceptFilterCollectionBuilder This => this;
    }

    public class RequestInterceptFilterCollection
        : BuilderCollectionBase<IRequestInterceptFilter>, IRequestInterceptFilterCollection
    {
        public RequestInterceptFilterCollection(Func<IEnumerable<IRequestInterceptFilter>> items)
            : base(items)
        { }

        // asynchronously evaluates all the filters and returns false if any of them return false
        public async ValueTask<bool> EvaluateCandidateAsync(Url url)
        {
            foreach (var filter in this)
            {
                if (!await filter.EvaluateCandidateAsync(url)) return false;
            }
            return true;
        }
    }
}
