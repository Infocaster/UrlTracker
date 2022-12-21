using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Mapping;
using UrlTracker.Backoffice.UI.Controllers.Models.Extensions;
using UrlTracker.Backoffice.UI.Extensions;

namespace UrlTracker.Backoffice.UI.Map
{
    internal class ExtensionMap : IMapDefinition
    {
        public void DefineMaps(IUmbracoMapper mapper)
        {
            mapper.Define<IUrlTrackerDashboardPage, DashboardPagesResponsePage>(
                (source, context) => new DashboardPagesResponsePage(source.Alias, source.View));
        }
    }
}
