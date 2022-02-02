using System.Linq;
using System.Web.Http;
using Swashbuckle.Application;
using Umbraco.Core.Composing;
using UrlTracker.Resources.Website.SwaggerExtensions.DocumentFilters;
using UrlTracker.Resources.Website.SwaggerExtensions.OperationFilters;

namespace UrlTracker.Resources.Website.Composing
{
#if !DEBUG
    [Disable]
#endif
    public class SwaggerComposer : ComponentComposer<SwaggerComponent>
    { }

    public class SwaggerComponent : IComponent
    {
        public void Initialize()
        {
            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "UrlTracker Test Api");
                    c.ResolveConflictingActions(a => a.First());
                    c.DocumentFilter<SwaggerDocumentFilter>();
                    c.OperationFilter<ApplySwaggerSummaryFilterAttributes>();
                    c.OperationFilter<IgnorePrefixParamsNameBeforeDot>();
                    c.UseFullTypeNameInSchemaIds();
                    c.DescribeAllEnumsAsStrings();
                })
                .EnableSwaggerUi();
        }

        // implements IComponent, but no termination logic is required
        public void Terminate()
        { }
    }
}