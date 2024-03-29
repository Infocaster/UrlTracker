﻿using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using UrlTracker.Backoffice.UI.Controllers.Models;

namespace UrlTracker.Backoffice.UI.Controllers.ActionFilters
{
    internal sealed class PatchModelAttribute
        : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
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
