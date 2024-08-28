using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlTracker.Modules.Options
{
    /// <summary>
    /// A model that represents an installed module of the URL Tracker
    /// </summary>
    /// <param name="Name">The name of the module</param>
    /// <param name="Icon">An icon for the module</param>
    public record UrlTrackerModuleRegistration(string Name, string? Icon);
}
