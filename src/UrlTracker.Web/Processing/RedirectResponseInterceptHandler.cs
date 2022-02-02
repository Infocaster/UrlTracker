using System.Net;
using System.Threading.Tasks;
using System.Web;
using Umbraco.Core.Mapping;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Models;
using UrlTracker.Web.Abstractions;
using ILogger = UrlTracker.Core.Logging.ILogger;

namespace UrlTracker.Web.Processing
{
    public class RedirectResponseInterceptHandler
        : ResponseInterceptHandlerBase<ShallowRedirect>
    {
        private readonly ILogger _logger;
        private readonly UmbracoMapper _mapper;
        private readonly ICompleteRequestAbstraction _completeRequestAbstraction;

        public RedirectResponseInterceptHandler(ILogger logger,
                                                UmbracoMapper mapper,
                                                ICompleteRequestAbstraction completeRequestAbstraction)
        {
            _logger = logger;
            _mapper = mapper;
            _completeRequestAbstraction = completeRequestAbstraction;
        }

        protected override ValueTask HandleAsync(HttpContextBase context, ShallowRedirect intercept)
        {
            if (!ShouldIntercept(context, intercept))
            {
                _logger.LogInterceptCancelled<RedirectResponseInterceptHandler>("requirements not met.", intercept);
                return new ValueTask();
            }

            // get actual redirect url and write to the response
            string url = GetUrl(context, intercept);

            var response = context.Response;
            response.Clear();
            if (!(url is null))
            {
                response.RedirectLocation = url;
            }
            response.StatusCode = url is null ? 410 : ((int)intercept.TargetStatusCode);
            context.CompleteRequest(_completeRequestAbstraction);
            _logger.LogInterceptPerformed<RedirectResponseInterceptHandler>(url);
            return new ValueTask();
        }

        private string GetUrl(HttpContextBase context, ShallowRedirect intercept)
        {
            Url url = _mapper.MapToUrl(intercept, context);
            var result = url?.ToString(UrlType.Absolute);
            return result;
        }

        public bool ShouldIntercept(HttpContextBase context, ShallowRedirect intercept)
        {
            return intercept.Force
                || context.Response.StatusCode == (int)HttpStatusCode.NotFound;
        }
    }
}
