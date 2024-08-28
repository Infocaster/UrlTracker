using System.Collections.Generic;

namespace UrlTracker.GlobalBlocklist.Services
{
    public class DenyListResponse
    {
        public List<string> GlobalBlocklist { get; set; }
    }
}
