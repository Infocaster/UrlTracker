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

                    actionContext.ActionArguments[key] = redirectRequestModel;
                }
            }
        }
    }
}
