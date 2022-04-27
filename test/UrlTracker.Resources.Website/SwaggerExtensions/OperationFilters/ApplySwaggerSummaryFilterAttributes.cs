using System.Linq;
using System.Web.Http.Description;
using Swashbuckle.Swagger;
using UrlTracker.Resources.Website.SwaggerExtensions.Attributes;

namespace UrlTracker.Resources.Website.SwaggerExtensions.OperationFilters
{
    public class ApplySwaggerSummaryFilterAttributes : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            var attr = apiDescription.GetControllerAndActionAttributes<SwaggerSummaryAttribute>().FirstOrDefault();
            if (attr != null)
            {
                operation.description = attr.Summary;
            }
        }
    }
}