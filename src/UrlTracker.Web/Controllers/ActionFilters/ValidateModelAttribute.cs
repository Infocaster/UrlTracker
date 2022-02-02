using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using UrlTracker.Core;

namespace UrlTracker.Web.Controllers.ActionFilters
{
    public class ValidateModelAttribute
        : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, actionContext.ModelState);
                return;
            }

            var parameters = actionContext.ActionDescriptor.GetParameters();
            foreach (var parameter in parameters)
            {
                if (parameter.IsOptional) continue;
                bool isValid = true;
                if (actionContext.ActionArguments[parameter.ParameterName] == parameter.DefaultValue)
                {
                    isValid = false;
                }

                if (isValid) continue;

                string message = (parameter.ParameterBinderAttribute is FromBodyAttribute)
                    ? Defaults.Validation.BodyContentMissing
                    : Defaults.Validation.QueryParamsMissing;

                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, message);
                return;
            }
        }
    }
}
