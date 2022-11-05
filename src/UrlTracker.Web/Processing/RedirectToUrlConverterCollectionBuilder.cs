using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core.Composing;
using UrlTracker.Core.Models;

namespace UrlTracker.Web.Processing
{
    [ExcludeFromCodeCoverage]
    public class RedirectToUrlConverterCollectionBuilder
        : OrderedCollectionBuilderBase<RedirectToUrlConverterCollectionBuilder, RedirectToUrlConverterCollection, IRedirectToUrlConverter>
    {
        /// <inheritdoc />
        protected override RedirectToUrlConverterCollectionBuilder This => this;
    }

    public interface IRedirectToUrlConverterCollection
    {
        string? Convert(Redirect redirect, HttpContext context);
    }

    [ExcludeFromCodeCoverage]
    public class RedirectToUrlConverterCollection : BuilderCollectionBase<IRedirectToUrlConverter>, IRedirectToUrlConverterCollection
    {
        public RedirectToUrlConverterCollection(Func<IEnumerable<IRedirectToUrlConverter>> items)
            : base(items)
        { }

        public string? Convert(Redirect redirect, HttpContext context)
        {
            var converter = this.First(c => c.CanHandle(redirect));
            return converter.Handle(redirect, context);
        }
    }
}
