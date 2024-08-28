using System.Runtime.Serialization;

namespace UrlTracker.Backoffice.UI.Controllers.Models.RedirectTarget
{
    [DataContract]
    internal class ContentTargetResponse
    {
        public ContentTargetResponse(string icon, string? iconColor, string name, string? url)
        {
            Icon = icon;
            IconColor = iconColor;
            Name = name;
            Url = url;
        }

        [DataMember(Name = "icon")]
        public string Icon { get; }

        [DataMember(Name = "iconColor")]
        public string? IconColor { get; }

        [DataMember(Name = "name")]
        public string Name { get; }
        
        [DataMember(Name = "url")]
        public string? Url { get; }
    };
}
