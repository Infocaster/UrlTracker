using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace UrlTracker.Backoffice.UI.Controllers.Swagger;

internal class UrlTrackerSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        options.SwaggerDoc(
            Defaults.Routing.SwaggerApi,
            new OpenApiInfo { Title = "URL Tracker Backoffice API", Version = "1.0" });

        options.OperationFilter<UrlTrackerSwaggerSecurityConfiguration>();
    }
}
