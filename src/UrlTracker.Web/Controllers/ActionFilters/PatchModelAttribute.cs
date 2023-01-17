using System;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using UrlTracker.Core;
using UrlTracker.Web.Controllers.Models;

namespace UrlTracker.Web.Controllers.ActionFilters
{
    public sealed class PatchModelAttribute
        : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            foreach (var key in actionContext.ActionArguments.Keys.ToList())
            {
                // TODO: extract into strategy pattern because the amount of rules is growing
                if (actionContext.ActionArguments[key] is RedirectRequestBase redirectRequestModel)
                {
                    if (!string.IsNullOrWhiteSpace(redirectRequestModel.OldUrl) &&
                        !redirectRequestModel.OldUrl.StartsWith("http") &&
                        char.IsLetterOrDigit(redirectRequestModel.OldUrl.First()))
                    {
                        redirectRequestModel.OldUrl = "/" + redirectRequestModel.OldUrl;
                    }

                    if (!string.IsNullOrWhiteSpace(redirectRequestModel.RedirectUrl) &&
                        !redirectRequestModel.RedirectUrl.StartsWith("http") &&
                        char.IsLetterOrDigit(redirectRequestModel.RedirectUrl.First()))
                    {
                        redirectRequestModel.RedirectUrl = "/" + redirectRequestModel.RedirectUrl;
                    }

                    if (!string.IsNullOrWhiteSpace(redirectRequestModel.Culture) &&
                        redirectRequestModel.Culture.Contains('-'))
                    {
                        redirectRequestModel.Culture = redirectRequestModel.Culture.NormalizeCulture();
                    }

                    redirectRequestModel.OldUrl = redirectRequestModel.OldUrl.DefaultIfNullOrWhiteSpace(null);
                    redirectRequestModel.OldRegex = redirectRequestModel.OldRegex.DefaultIfNullOrWhiteSpace(null);

                    actionContext.ActionArguments[key] = redirectRequestModel;
                }
            }
        }
    }
}
