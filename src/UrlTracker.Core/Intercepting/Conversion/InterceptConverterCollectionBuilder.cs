using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Umbraco.Core.Composing;
using UrlTracker.Core.Intercepting.Models;

namespace UrlTracker.Core.Intercepting.Conversion
{
    /* To improve performance, the output of the intermediate intercept service is always a cachable model.
     * By ensuring cachability of that model, we can easily introduce a caching layer on top of the intermediate service.
     * For some actions, a cachable model is too flat though and needs to be enriched with for example IPublishedContent data.
     * 
     * The intercept converter collection is introduced to convert cachable models to rich models and is used
     * in the api layer to serve rich models to callers.
     * 
     * Since any cachable intercept is by definition an intercept, if no explicit conversion exists, the model itself is
     * returned.
     */
    [ExcludeFromCodeCoverage]
    public class InterceptConverterCollectionBuilder
        : OrderedCollectionBuilderBase<InterceptConverterCollectionBuilder, InterceptConverterCollection, IInterceptConverter>
    {
        protected override InterceptConverterCollectionBuilder This => this;
    }

    public class InterceptConverterCollection
        : BuilderCollectionBase<IInterceptConverter>, IInterceptConverterCollection
    {
        public InterceptConverterCollection(IEnumerable<IInterceptConverter> items) : base(items)
        { }

        public async ValueTask<IIntercept> ConvertAsync(ICachableIntercept cachableIntercept)
        {
            foreach (var converter in this)
            {
                var result = await converter.ConvertAsync(cachableIntercept);
                if (result != null) return result;
            }

            return cachableIntercept;
        }
    }
}
