﻿using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UrlTracker.Core.Intercepting.Models;

namespace UrlTracker.Web.Processing
{
    [ExcludeFromCodeCoverage]
    public abstract class ResponseInterceptHandlerBase<TInput>
        : IResponseInterceptHandler
    {
        public virtual bool CanHandle(IIntercept intercept)
            => intercept.Info is TInput;

        public ValueTask HandleAsync(RequestDelegate next, HttpContext context, IIntercept intercept)
            => intercept.Info is TInput realIntercept ? HandleAsync(next, context, realIntercept) : new ValueTask();

        protected abstract ValueTask HandleAsync(RequestDelegate next, HttpContext context, TInput intercept);
    }
}
