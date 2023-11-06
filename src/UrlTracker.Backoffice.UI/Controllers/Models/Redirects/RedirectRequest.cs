using System.Runtime.Serialization;
using UrlTracker.Backoffice.UI.Controllers.Models.Base;

namespace UrlTracker.Backoffice.UI.Controllers.Models.Redirects
{
    [DataContract]
    internal class RedirectRequest
        : RedirectViewModelBase
    { }

    internal class RedirectBulkRequest
    {
        public int Id { get; set; }
        public RedirectRequest Redirect { get; set; }
    }
}
