using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Composing;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Core.Models;

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
        private readonly ILastChanceInterceptor _lastChance;

        public InterceptorCollection(Func<IEnumerable<IInterceptor>> items, ILastChanceInterceptor lastChance)
            : base(items)
        {
            _lastChance = lastChance;
        }

        public async ValueTask<ICachableIntercept> InterceptAsync(Url url, IInterceptContext context)
        {
            foreach (var interceptor in this)
            {
                var result = await interceptor.InterceptAsync(url, context);
                if (result is not null) return result;
            }

            return await _lastChance.InterceptAsync(url, context);
        }
    }
}
