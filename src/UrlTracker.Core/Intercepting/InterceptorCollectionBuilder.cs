using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Composing;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Intercepting.Models;

namespace UrlTracker.Core.Intercepting
{
    [ExcludeFromCodeCoverage]
    public class InterceptorCollectionBuilder
        : OrderedCollectionBuilderBase<InterceptorCollectionBuilder, InterceptorCollection, IInterceptor>
    {
        protected override InterceptorCollectionBuilder This => this;
    }

    public class InterceptorCollection
        : BuilderCollectionBase<IInterceptor>, IInterceptorCollection
    {
        public InterceptorCollection(Func<IEnumerable<IInterceptor>> items)
            : base(items)
        { }

        public async ValueTask<ICachableIntercept?> InterceptAsync(Url url, IReadOnlyInterceptContext context)
        {
            foreach (var interceptor in this)
            {
                var result = await interceptor.InterceptAsync(url, context);
                if (result is not null) return result;
            }

            return null;
        }
    }
}
