using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Api.Management.OpenApi;

namespace UrlTracker.Backoffice.UI.Controllers.Swagger
{
    internal class UrlTrackerBackofficeRequirementFilter
        : BackOfficeSecurityRequirementsOperationFilterBase
    {
        protected override string ApiName { get; } = Defaults.Routing.V1.ApiName;
    }
}
