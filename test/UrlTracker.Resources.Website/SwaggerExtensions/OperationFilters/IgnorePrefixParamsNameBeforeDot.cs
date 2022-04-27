using System.Web.Http.Description;
using Swashbuckle.Swagger;

namespace UrlTracker.Resources.Website.SwaggerExtensions.OperationFilters
{
    public class IgnorePrefixParamsNameBeforeDot : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.parameters == null) return;

            foreach (Parameter param in operation.parameters)
            {
                var index = param.name.IndexOf('.');
                if (index != -1)
                {
                    index++;
                    param.name = param.name.Substring(index, param.name.Length - index);
                }
            }
        }
    }
}