using System.Collections.Generic;
using System.Runtime.Serialization;
using UrlTracker.Backoffice.UI.Controllers.Models.Base;

namespace UrlTracker.Backoffice.UI.Controllers.Models.Extensions
{
    [DataContract]
    internal class DashboardPagesResponse
        : CollectionResponseBase<DashboardPagesResponsePage>
    {
        public DashboardPagesResponse(IEnumerable<DashboardPagesResponsePage> results)
            : base(results)
        { }
    }

    [DataContract]
    internal class DashboardPagesResponsePage
    {
        public DashboardPagesResponsePage(string alias, string view)
        {
            Alias = alias;
            View = view;
        }

        [DataMember(Name = "alias")]
        public string Alias { get; set; }

        [DataMember(Name = "view")]
        public string View { get; set; }
    }
}
