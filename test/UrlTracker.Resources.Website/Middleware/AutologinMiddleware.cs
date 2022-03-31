using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.BackOffice.Security;
using UrlTracker.Resources.Website.Extensions;

namespace UrlTracker.Resources.Website.Middleware
{
    public class AutoLoginMiddleware
    {
        private readonly RequestDelegate _next;

        public AutoLoginMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, IBackOfficeSignInManager signInManager, IBackOfficeUserManager backOfficeUserManager, IUmbracoContextAccessor umbracoContextAccessor)
        {
            //if PublishedRequest is null, request is not from frontend
            if (!IsAuthenticated(httpContext) && IsBackofficeRequest(umbracoContextAccessor) && RequestIsLocal(httpContext))
            {
                //login default user
                var user = await backOfficeUserManager.FindByIdAsync(Constants.Security.SuperUserIdAsString);
                await signInManager.SignInAsync(user, true);
            }

            await _next(httpContext);
        }

        private static bool IsBackofficeRequest(IUmbracoContextAccessor umbracoContextAccessor)
        {
            return umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext) && umbracoContext.PublishedRequest is null;
        }

        private static bool IsAuthenticated(HttpContext httpContext)
        {
            return httpContext.User.Identity?.IsAuthenticated ?? false;
        }

        private static bool RequestIsLocal(HttpContext httpContext)
        {
            var remoteAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

            // The requester may automatically sign in if the connect from a local ip or a local network ip
            //    (read as: connect from same device or device in infocaster network)
            if (httpContext.Request.IsLocal() || remoteAddress.StartsWith("192.168"))
            {
                return true;
            }
            return false;
        }
    }

    public static class AutomatedBackOfficeLoginExtensions
    {
        public static IApplicationBuilder UseAutomatedBackOfficeAuthentication(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AutoLoginMiddleware>();
        }
    }
}
