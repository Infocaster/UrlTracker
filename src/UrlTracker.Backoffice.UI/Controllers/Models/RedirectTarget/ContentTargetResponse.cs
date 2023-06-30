using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UrlTracker.Backoffice.UI.Controllers.Models.RedirectTarget
{
    [DataContract]
    internal class ContentTargetResponse
    {
        public ContentTargetResponse(string icon, string name)
        {
            Icon = icon;
            Name = name;
        }

        [DataMember(Name = "icon")]
        public string Icon { get; }
        
        [DataMember(Name = "name")]
        public string Name { get; }
    };
}
