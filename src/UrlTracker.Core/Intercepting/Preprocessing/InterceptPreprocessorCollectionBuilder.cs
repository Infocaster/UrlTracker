using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Composing;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Intercepting.Models;

namespace UrlTracker.Core.Intercepting.Preprocessing
{
    [ExcludeFromCodeCoverage]
    public class InterceptPreprocessorCollectionBuilder
        : OrderedCollectionBuilderBase<InterceptPreprocessorCollectionBuilder, InterceptPreprocessorCollection, IInterceptPreprocessor>
    {
        protected override InterceptPreprocessorCollectionBuilder This => this;
    }

    public class InterceptPreprocessorCollection
        : BuilderCollectionBase<IInterceptPreprocessor>, IInterceptPreprocessorCollection
    {
        private readonly IDefaultInterceptContextFactory _contextFactory;

        public InterceptPreprocessorCollection(Func<IEnumerable<IInterceptPreprocessor>> items, IDefaultInterceptContextFactory contextFactory)
            : base(items)
        {
            _contextFactory = contextFactory;
        }

        public async ValueTask<IInterceptContext> PreprocessUrlAsync(Url url, IInterceptContext? context = null)
        {
            context = context ?? _contextFactory.Create();
            foreach (var preprocessor in this)
            {
                context = await preprocessor.PreprocessUrlAsync(url, context);
            }
            return context;
        }
    }
}
