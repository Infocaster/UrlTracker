using System.Web;
using Umbraco.Core.IO;
using Umbraco.Web;
using Umbraco.Web.Security;

namespace UrlTracker.Resources.Website
{
    public class Global : UmbracoApplication
    {
        private bool _requestIsLocal => Request.UserHostAddress.StartsWith("192.168") || Request.UserHostAddress.StartsWith("10.") || Request.UserHostAddress == "127.0.0.1";

        protected void Application_AuthenticateRequest()
        {
#if DEBUG
            // Auto login user if debug on local machine
            var context = Umbraco.Web.Composing.Current.UmbracoContext;

            if (context != null && !context.IsFrontEndUmbracoRequest && Request.AppRelativeCurrentExecutionFilePath.StartsWith(SystemDirectories.Umbraco))
            {
                var auth = new HttpContextWrapper(HttpContext.Current).GetUmbracoAuthTicket();
                if (auth != null)
                {
                    var userService = Umbraco.Web.Composing.Current.Services.UserService;
                    var currentUser = userService.GetByUsername(auth.Identity.Name);

                    if (currentUser != null)
                        return;
                }

                if (_requestIsLocal)
                {
                    context.Security.PerformLogin(-1);
                    Response.Redirect(Request.RawUrl);
                }
            }
#endif
        }
    }
}