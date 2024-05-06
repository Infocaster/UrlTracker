using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Core.Logging;

namespace UrlTracker.Web.Processing.Handling
{
    [ExcludeFromCodeCoverage]
    public class LastChanceResponseInterceptHandler
        : ILastChanceResponseInterceptHandler
    {
        private readonly ILogger<LastChanceResponseInterceptHandler> _logger;

        public LastChanceResponseInterceptHandler(ILogger<LastChanceResponseInterceptHandler> logger)
        {
            _logger = logger;
        }

        public ValueTask HandleAsync(RequestDelegate next, HttpContext context, IIntercept intercept)
        {
            _logger.LogLastChance(intercept.GetType());
            return new ValueTask(next(context));
        }
    }
}
