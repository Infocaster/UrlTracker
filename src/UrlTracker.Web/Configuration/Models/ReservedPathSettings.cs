using System.Collections.Generic;
using System.Linq;

namespace UrlTracker.Web.Configuration.Models
{
    public class ReservedPathSettings
    {
        private ISet<string> _paths;

        public ISet<string> Paths
        {
            get => _paths; set
            {
                // ensure that paths are in a unified format. This makes comparison easy and efficient
                _paths = new HashSet<string>(from path in value
                                             where !string.IsNullOrWhiteSpace(path)
                                             select path.Trim(' ', '~', '/', '\\') + '/');
            }
        }
    }
}
