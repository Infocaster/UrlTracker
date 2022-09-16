using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
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
                        var splitIndex = redirectRequestModel.Culture.IndexOf('-') + 1;
                        redirectRequestModel.Culture = redirectRequestModel.Culture.Substring(0, splitIndex) + redirectRequestModel.Culture.Substring(splitIndex).ToUpper();
                    }

                    actionContext.ActionArguments[key] = redirectRequestModel;
                }
            }
        }
    }
}
