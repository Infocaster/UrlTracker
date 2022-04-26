using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Web;
using UrlTracker.Core.Intercepting.Models;
using ILogger = UrlTracker.Core.Logging.ILogger;

namespace UrlTracker.Web.Processing
{
    [ExcludeFromCodeCoverage]
    public class LastChanceResponseInterceptHandler
        : ILastChanceResponseInterceptHandler
    {
        private readonly ILogger _logger;
        public LastChanceResponseInterceptHandler(ILogger logger)
        {
            _logger = logger;
        }
        public ValueTask HandleAsync(HttpContextBase context, IIntercept intercept)
        {
            _logger.LogLastChance<LastChanceResponseInterceptHandler>(intercept.GetType());
            return new ValueTask();
        }
    }
}
