using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UrlTracker.Backoffice.UI.Controllers.Models.Redirects
{
    [DataContract]
    internal class CreateRedirectRequest
        : RedirectRequest
    {
        [DataMember(Name = "solvedRecommendation")]
        public int? SolvedRecommendation { get; set; }
    }
}
