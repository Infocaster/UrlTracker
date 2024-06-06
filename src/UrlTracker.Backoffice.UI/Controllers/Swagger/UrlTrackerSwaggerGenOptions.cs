using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace UrlTracker.Backoffice.UI.Controllers.Swagger
{
    internal class UrlTrackerSwaggerGenOptions
        : IConfigureOptions<SwaggerGenOptions>
    {
        public void Configure(SwaggerGenOptions options)
        {
            options.SwaggerDoc(Defaults.Routing.V1.ApiName, new OpenApiInfo
            {
                Title = "URL Tracker API V1",
                Description = "This API is used by the UI of the URL Tracker plugin in the backoffice",
                Version = Defaults.Routing.V1.ApiVersion
            });
            options.OperationFilter<UrlTrackerBackofficeRequirementFilter>();
        }
    }
}
