using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;
using UrlTracker.Core;

namespace UrlTracker.Web.Controllers.ActionFilters
{
    // public class ValidateModelAttribute
    //     : ActionFilterAttribute
    // {
    //     public override void OnActionExecuting(ActionExecutingContext actionContext)
    //     {
    //         if (!actionContext.ModelState.IsValid)
    //         {
    //             actionContext.HttpContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, actionContext.ModelState);
    //             return;
    //         }

    //         var parameters = actionContext.ActionDescriptor.GetParameters();
    //         foreach (var parameter in parameters)
    //         {
    //             if (parameter.IsOptional) continue;
    //             bool isValid = true;
    //             if (actionContext.ActionArguments[parameter.ParameterName] == parameter.DefaultValue)
    //             {
    //                 isValid = false;
    //             }

    //             if (isValid) continue;

    //             string message = (parameter.ParameterBinderAttribute is FromBodyAttribute)
    //                 ? Defaults.Validation.BodyContentMissing
    //                 : Defaults.Validation.QueryParamsMissing;

    //             actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, message);
    //             return;
    //         }
    //     }
    // }
}
