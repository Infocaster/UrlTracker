using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.ApplicationBuilder;
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
        private readonly IRuntimeState _runtimeState;

        public UrlTrackerMiddleware(RequestDelegate next,
                                    ILogger<UrlTrackerMiddleware> logger,
                                    IInterceptService interceptService,
                                    IResponseInterceptHandlerCollection interceptHandlers,
                                    IRequestInterceptFilterCollection requestFilters,
                                    IEventAggregator eventAggregator,
                                    IRuntimeState runtimeState)
        {
            _next = next;
            _logger = logger;
            _interceptService = interceptService;
            _interceptHandlers = interceptHandlers;
            _requestFilters = requestFilters;
            _eventAggregator = eventAggregator;
            _runtimeState = runtimeState;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // ignore this middleware as long as umbraco hasn't been initialised yet
            if (_runtimeState.Level < RuntimeLevel.Run)
            {
                await _next(context);
                return;
            }

            Url url = context.Request.GetUrl();
            _logger.LogRequestDetected(url.ToString());

            await DoInterceptAsync(context, url);
            await _eventAggregator.PublishAsync(new UrlTrackerHandled(context, url));
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

            _logger.LogInterceptFound(intercept.GetType());

            var handler = _interceptHandlers.Get(intercept);
            await handler.HandleAsync(_next, context, intercept);
        }
    }

    [ExcludeFromCodeCoverage]
    public class ConfigurePipelineOptions : IConfigureOptions<UmbracoPipelineOptions>
    {
        public void Configure(UmbracoPipelineOptions options)
        {
            options.AddFilter(new UrlTrackerStartupFilter());
        }
    }

    [ExcludeFromCodeCoverage]
    public class UrlTrackerStartupFilter : IUmbracoPipelineFilter
    {
        public string Name => "URL Tracker";

        public void OnEndpoints(IApplicationBuilder app)
        { }

        public void OnPostPipeline(IApplicationBuilder app)
        {
            app.UseMiddleware<UrlTrackerMiddleware>();
        }

        public void OnPrePipeline(IApplicationBuilder app)
        { }
    }

    [ExcludeFromCodeCoverage]
    public static class IApplicationBuilderExtensions
    {
        [Obsolete("This call is no longer required as the middleware is now automatically registered. This method in fact doesn't do anything anymore.")]
        public static IApplicationBuilder UseUrlTracker(this IApplicationBuilder app)
        {
            // Leave this method up for the next major release to allow users some space to adapt to the change
            return app;
        }

        [Obsolete("This call is no longer required as the middleware is now automatically registered. This method in fact doesn't do anything anymore.")]
        public static IUmbracoApplicationBuilderContext UseUrlTracker(this IUmbracoApplicationBuilderContext app)
        {
            // Leave this method up for the next major release to allow users some space to adapt to the change
            return app;
        }
    }
}
