﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Web;
using UrlTracker.Core;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Logging;
using UrlTracker.Web.Events.Models;
using UrlTracker.Web.Processing;

namespace UrlTracker.Web
{
    public class UrlTrackerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UrlTrackerMiddleware> _logger;
        private readonly IInterceptService _interceptService;
        private readonly IResponseInterceptHandlerCollection _interceptHandlers;
        private readonly IRequestInterceptFilterCollection _requestFilters;
        private readonly IEventAggregator _eventAggregator;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        public UrlTrackerMiddleware(RequestDelegate next,
                                    ILogger<UrlTrackerMiddleware> logger,
                                    IInterceptService interceptService,
                                    IResponseInterceptHandlerCollection interceptHandlers,
                                    IRequestInterceptFilterCollection requestFilters,
                                    IEventAggregator eventAggregator)
        {
            _next = next;
            _logger = logger;
            _interceptService = interceptService;
            _interceptHandlers = interceptHandlers;
            _requestFilters = requestFilters;
            _eventAggregator = eventAggregator;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Url url = context.Request.GetUrl();
            _logger.LogRequestDetected(url.ToString());

            await DoInterceptAsync(context, url);
            await _eventAggregator.PublishAsync(new UrlTrackerHandled(context));
        }

        public async Task DoInterceptAsync(HttpContext context, Url url)
        {
            if (!await _requestFilters.EvaluateCandidateAsync(url))
            {
                _logger.LogAbortHandling("Incoming request failed intercept candidacy check.");
                await _next(context);
                return;
            }

            var intercept = await _interceptService.GetAsync(url);
            if (intercept is null)
            {
                _logger.LogAbortHandling("No intercept was found");
                await _next(context);
                return;
            }

            _logger.LogInterceptFound(intercept.GetType());

            var handler = _interceptHandlers.Get(intercept);
            await handler.HandleAsync(_next, context, intercept);
        }
    }

    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseUrlTracker(this IApplicationBuilder app)
        {
            app.UseMiddleware<UrlTrackerMiddleware>();
            return app;
        }
    }
}
