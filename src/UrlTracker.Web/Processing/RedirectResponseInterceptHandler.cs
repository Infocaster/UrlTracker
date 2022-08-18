using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Mapping;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Models;
using UrlTracker.Web.Abstractions;
using ILogger = UrlTracker.Core.Logging.ILogger;

namespace UrlTracker.Web.Processing
{
    public class RedirectResponseInterceptHandler
        : ResponseInterceptHandlerBase<Redirect>
    {
        private readonly ILogger _logger;
        private readonly UmbracoMapper _mapper;
        private readonly ICompleteRequestAbstraction _completeRequestAbstraction;
        private readonly IUmbracoSettingsSection _umbracoSettingsSection;

        public RedirectResponseInterceptHandler(ILogger logger,
                                                UmbracoMapper mapper,
                                                ICompleteRequestAbstraction completeRequestAbstraction,
                                                IUmbracoSettingsSection umbracoSettingsSection)
        {
            _logger = logger;
            _mapper = mapper;
            _completeRequestAbstraction = completeRequestAbstraction;
            _umbracoSettingsSection = umbracoSettingsSection;
        }

        protected override ValueTask HandleAsync(HttpContextBase context, Redirect intercept)
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
                // If redirect is a regex match, ensure that potential capture tokens are replaced in the target url
                // TODO: Evaluate side effects!
                //    This logic has been taken from the old code base. It has a potential side effect.
                //    If a pattern matches on a partial string, the non-matched part will stay in the url
                //    example: regex:"(ipsum)" targeturl: "http://example.com/$1" input: "lorem/ipsum/dolor" result: "lorem/http://example.com/ipsum/dolor"
                if (string.IsNullOrWhiteSpace(intercept.SourceUrl) && !string.IsNullOrWhiteSpace(intercept.SourceRegex))
                {
                    url = Regex.Replace(context.Request.Url.PathAndQuery.TrimStart('/'), intercept.SourceRegex, url);
                }
                response.RedirectLocation = url;
            }
            response.StatusCode = url is null ? 410 : ((int)intercept.TargetStatusCode);
            context.CompleteRequest(_completeRequestAbstraction);
            _logger.LogInterceptPerformed<RedirectResponseInterceptHandler>(url);
            return new ValueTask();
        }

        private string GetUrl(HttpContextBase context, Redirect intercept)
        {
            Url url = _mapper.MapToUrl(intercept, context);
            var result = url?.ToString(UrlType.Absolute);
            return result;
        }

        public bool ShouldIntercept(HttpContextBase context, Redirect intercept)
        {
            return intercept.Force
                || context.Response.StatusCode == (int)HttpStatusCode.NotFound;
        }
    }
}
