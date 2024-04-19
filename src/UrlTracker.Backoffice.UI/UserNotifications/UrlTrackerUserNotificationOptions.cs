using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace UrlTracker.Backoffice.UI.UserNotifications
{
    /// <summary>
    /// This object contains a list of notifications that are to be shown to users in the backoffice
    /// </summary>
    public class UrlTrackerUserNotificationOptions
    {
        /// <inheritdoc />
        public UrlTrackerUserNotificationOptions()
        {
            Notifications = new List<UrlTrackerUserNotification>();
        }

        /// <summary>
        /// The list of notifications
        /// </summary>
        public ICollection<UrlTrackerUserNotification> Notifications { get; set; }
    }

    /// <summary>
    /// A notification that can be shown to the user of the URL Tracker
    /// </summary>
    /// <param name="Id"> Any short string that is unique to this notification. </param>
    /// <param name="TranslatableTitleComponent"> A string that references a translation in a translation file. For example: "urltrackernotifications_newversiontitle" </param>
    /// <param name="TitleArguments"> The string arguments that need to be passed to the translator while building the translated string </param>
    /// <param name="TranslatableBodyComponent"> A string that references a translation in a translation file. For example: "urltrackernotifications_newversionbody" </param>
    /// <param name="BodyArguments"> The string arguments that need to be passed to the translator while building the translated string </param>
    public record UrlTrackerUserNotification(
        string Id,
        string TranslatableTitleComponent,
        ICollection<string> TitleArguments,
        string TranslatableBodyComponent,
        ICollection<string> BodyArguments);
}
