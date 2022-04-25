using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Umbraco.Cms.Core.Composing;
using UrlTracker.Core.Intercepting.Models;

namespace UrlTracker.Web.Processing
{
    [ExcludeFromCodeCoverage]
    public class ResponseInterceptHandlerCollectionBuilder
        : OrderedCollectionBuilderBase<ResponseInterceptHandlerCollectionBuilder, ResponseInterceptHandlerCollection, ISpecificResponseInterceptHandler>
    {
        protected override ResponseInterceptHandlerCollectionBuilder This => this;
    }

    public class ResponseInterceptHandlerCollection
        : BuilderCollectionBase<ISpecificResponseInterceptHandler>, IResponseInterceptHandlerCollection
    {
        private readonly ILastChanceResponseInterceptHandler _lastChance;

        public ResponseInterceptHandlerCollection(Func<IEnumerable<ISpecificResponseInterceptHandler>> items,
                                                  ILastChanceResponseInterceptHandler lastChance)
            : base(items)
        {
            _lastChance = lastChance;
        }

        public IResponseInterceptHandler Get(IIntercept intercept)
        {
            return this.FirstOrDefault(handler => handler.CanHandle(intercept))
                ?? (IResponseInterceptHandler)_lastChance;
        }
    }
}
