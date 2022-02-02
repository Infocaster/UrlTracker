using System.Diagnostics.CodeAnalysis;

namespace UrlTracker.Core.Configuration.Models
{
    [ExcludeFromCodeCoverage]
    public class UrlTrackerSettings
    {
        public UrlTrackerSettings(bool isDisabled, bool isTrackingDisabled, bool loggingEnabled, bool isNotFoundTrackingDisabled, bool appendPortNumber, bool hasDomainOnChildNode)
        {
            IsDisabled = isDisabled;
            IsTrackingDisabled = isTrackingDisabled;
            LoggingEnabled = loggingEnabled;
            IsNotFoundTrackingDisabled = isNotFoundTrackingDisabled;
            AppendPortNumber = appendPortNumber;
            HasDomainOnChildNode = hasDomainOnChildNode;
        }

        /// <summary>
        /// Returns wether or not the UrlTracker is disabled
        /// </summary>
        /// <remarks>
        /// appSetting: 'urlTracker:disabled'
        /// </remarks>
        public bool IsDisabled { get; }

        /// <summary>
        /// Returns wether or not tracking URL changes is disabled
        /// </summary>
        /// <remarks>
        /// appSetting: 'urlTracker:trackingDisabled'
        /// </remarks>
        public bool IsTrackingDisabled { get; }

        /// <summary>
        /// Returns wether or not logging for the UrlTracker is enabled
        /// </summary>
        /// <remarks>
        /// appSettings: 'urlTracker:enableLogging' & 'umbracoDebugMode'
        /// </remarks>
        public bool LoggingEnabled { get; }

        /// <summary>
        /// Returns wether or not not found (404) tracking is disabled
        /// </summary>
        /// <remarks>
        /// appSetting: 'urlTracker:notFoundTrackingDisabled'
        /// </remarks>
        public bool IsNotFoundTrackingDisabled { get; }

        /// <summary>
        /// Gets a value indicating whether or not to append port numbers to URLs. Default is true.
        /// </summary>
        /// <value>
        ///   <c>true</c> if we are to append the port number; otherwise, <c>false</c>.
        /// </value>
        public bool AppendPortNumber { get; }

        /// <summary>
        /// Returns wether or not a child node has a domain configured
        /// </summary>
        /// <remarks>
        /// appSetting: 'urlTracker:hasDomainOnChildNode'
        /// </remarks>
        public bool HasDomainOnChildNode { get; }
    }
}
