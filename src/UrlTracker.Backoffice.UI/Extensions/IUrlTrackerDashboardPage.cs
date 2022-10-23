namespace UrlTracker.Backoffice.UI.Extensions
{
    /// <summary>
    /// When implemented, this type defines a page on the URL Tracker dashboard
    /// </summary>
    public interface IUrlTrackerDashboardPage
    {
        /// <summary>
        /// A unique alias that defines this page
        /// </summary>
        /// <remarks>
        /// <para>This alias is also used as translation key</para>
        /// </remarks>
        string Alias { get; }

        /// <summary>
        /// The path to an angularjs view that represents this page
        /// </summary>
        string View { get; }
    }
}
