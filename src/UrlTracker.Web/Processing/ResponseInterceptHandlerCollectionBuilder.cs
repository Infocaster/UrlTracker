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
        : OrderedCollectionBuilderBase<ResponseInterceptHandlerCollectionBuilder, ResponseInterceptHandlerCollection, IResponseInterceptHandler>
    {
        protected override ResponseInterceptHandlerCollectionBuilder This => this;
    }

    public class ResponseInterceptHandlerCollection
        : BuilderCollectionBase<IResponseInterceptHandler>, IResponseInterceptHandlerCollection
    {
        public ResponseInterceptHandlerCollection(Func<IEnumerable<IResponseInterceptHandler>> items)
            : base(items)
        { }

        public IResponseInterceptHandler Get(IIntercept intercept)
        {
            return this.FirstOrDefault(handler => handler.CanHandle(intercept));
        }
    }
}
