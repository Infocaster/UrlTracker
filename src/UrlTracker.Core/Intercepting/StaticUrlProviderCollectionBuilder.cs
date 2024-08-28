using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Umbraco.Cms.Core.Composing;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Intercepting
{
    [ExcludeFromCodeCoverage]
    public class StaticUrlProviderCollectionBuilder
        : OrderedCollectionBuilderBase<StaticUrlProviderCollectionBuilder, StaticUrlProviderCollection, IStaticUrlProvider>
    {
        protected override StaticUrlProviderCollectionBuilder This => this;
    }

    public class StaticUrlProviderCollection
        : BuilderCollectionBase<IStaticUrlProvider>, IStaticUrlProviderCollection
    {
        public StaticUrlProviderCollection(Func<IEnumerable<IStaticUrlProvider>> items)
            : base(items)
        { }

        public IEnumerable<string> GetUrls(Url url)
            => this.SelectMany(p => p.Get(url)).Distinct().ToList();
    }
}
