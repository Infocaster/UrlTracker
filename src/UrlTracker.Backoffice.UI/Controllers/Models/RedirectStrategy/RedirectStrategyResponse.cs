using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UrlTracker.Backoffice.UI.Controllers.Models.RedirectStrategy
{
    [DataContract]
    internal record RedirectStrategyResponse(
        [property: DataMember(Name = "name")] string Name,
        [property: DataMember(Name = "key")] Guid Key
        );
}
