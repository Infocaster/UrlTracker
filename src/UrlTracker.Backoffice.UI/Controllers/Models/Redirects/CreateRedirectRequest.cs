using System;
using System.Runtime.Serialization;

namespace UrlTracker.Backoffice.UI.Controllers.Models.Redirects
{
    [DataContract]
    internal class CreateRedirectRequest
        : RedirectRequest
    {
        [DataMember(Name = "key")]
        public Guid? Key { get; set; }

        [DataMember(Name = "solvedRecommendation")]
        public int? SolvedRecommendation { get; set; }
    }
}
