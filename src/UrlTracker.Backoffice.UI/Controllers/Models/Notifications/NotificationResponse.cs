using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UrlTracker.Backoffice.UI.Controllers.Models.Notifications
{
    [DataContract]
    internal record NotificationResponse(
        [property: DataMember(Name = "id")] string Id,
        [property: DataMember(Name = "translatableTitleComponent")] string TranslatableTitleComponent,
        [property: DataMember(Name = "titleArguments")] ICollection<string> TitleArguments,
        [property: DataMember(Name = "translatableBodyComponent")] string TranslatableBodyComponent,
        [property: DataMember(Name = "bodyArguments")] ICollection<string> BodyArguments);
}
