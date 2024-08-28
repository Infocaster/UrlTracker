using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlTracker.Modules.Options
{
    public class UrlTrackerModuleOptions
    {
        private List<UrlTrackerModuleRegistration> _installedModules;
        
        public IReadOnlyCollection<UrlTrackerModuleRegistration> InstalledModules { get => _installedModules; }

        public void RegisterModule(string name, string? icon = null)
        {
            _installedModules.Add(new UrlTrackerModuleRegistration(name, icon));
        }
    }
}
