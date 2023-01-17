using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public class UrlTrackerUserNotification
    {
        /// <summary>
        /// Any short string that is unique to this notification.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// A string that references a translation in a translation file. For example: "urltrackernotifications_newversiontitle"
        /// </summary>
        public string TranslatableTitleComponent { get; set; }

        /// <summary>
        /// The string arguments that need to be passed to the translator while building the translated string
        /// </summary>
        public ICollection<string> TitleArguments { get; set; }
        
        /// <summary>
        /// A string that references a translation in a translation file. For example: "urltrackernotifications_newversionbody"
        /// </summary>
        public string TranslatableBodyComponent { get; set; }

        /// <summary>
        /// The string arguments that need to be passed to the translator while building the translated string
        /// </summary>
        public ICollection<string> BodyArguments { get; set; }
    }
}
