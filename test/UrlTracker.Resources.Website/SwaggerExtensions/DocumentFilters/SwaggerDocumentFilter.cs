using System;
using System.Linq;
using System.Web.Http.Description;
using Swashbuckle.Swagger;

namespace UrlTracker.Resources.Website.SwaggerExtensions.DocumentFilters
{
    public class SwaggerDocumentFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            swaggerDoc.paths = swaggerDoc
                .paths
                .Where(x => x.Key.StartsWith("/umbraco/backoffice/urltracker", StringComparison.InvariantCultureIgnoreCase)
                         || x.Key.StartsWith("/api", StringComparison.InvariantCultureIgnoreCase))
                .ToDictionary(e => e.Key, e => e.Value);
        }
    }
}