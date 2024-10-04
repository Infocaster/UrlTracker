using System.Runtime.Serialization;
using UrlTracker.Backoffice.UI.Controllers.Models.Base;

namespace UrlTracker.Backoffice.UI.Controllers.Models.Redirects
{
    [DataContract]
    internal class RedirectRequest
        : RedirectViewModelBase
    { }

    internal class RedirectBulkRequest
        : EntityWithIdRequest<RedirectRequest>
    { }
}
