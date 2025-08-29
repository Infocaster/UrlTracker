using Umbraco.Cms.Api.Management.OpenApi;

namespace UrlTracker.Backoffice.UI.Controllers.Swagger;

internal class UrlTrackerSwaggerSecurityConfiguration : BackOfficeSecurityRequirementsOperationFilterBase
{
    protected override string ApiName => Defaults.Routing.SwaggerApi;
}
