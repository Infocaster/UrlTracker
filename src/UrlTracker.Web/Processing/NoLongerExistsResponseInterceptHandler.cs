﻿using System.Threading.Tasks;
using System.Web;
using UrlTracker.Core.Database.Models;
using UrlTracker.Web.Abstractions;

namespace UrlTracker.Web.Processing
{
    public class NoLongerExistsResponseInterceptHandler
        : ResponseInterceptHandlerBase<UrlTrackerShallowClientError>
    {
        private readonly ICompleteRequestAbstraction _completeRequestAbstraction;

        public NoLongerExistsResponseInterceptHandler(ICompleteRequestAbstraction completeRequestAbstraction)
        {
            _completeRequestAbstraction = completeRequestAbstraction;
        }

        protected override ValueTask HandleAsync(HttpContextBase context, UrlTrackerShallowClientError intercept)
        {
            if (!ShouldRedirect(context))
            {
                return new ValueTask();
            }

            var response = context.Response;
            response.Clear();
            response.StatusCode = 410;
            context.CompleteRequest(_completeRequestAbstraction);

            return new ValueTask();
        }

        private bool ShouldRedirect(HttpContextBase context)
        {
            return context.Response.StatusCode >= 400;
        }
    }
}