using System;
using System.Text.RegularExpressions;

namespace InfoCaster.Umbraco.UrlTracker.Settings
{
	public interface IUrlTrackerSettings
	{
		string GetTableName();
		string GetOldTableName();
		string GetHttpModuleCheck();
		bool IsSEOMetadataInstalled();
		string GetSEOMetadataPropertyName();

		/// <summary>
		/// Returns wether or not the UrlTracker is disabled
		/// </summary>
		/// <remarks>
		/// appSetting: 'urlTracker:disabled'
		/// </remarks>
		bool IsDisabled();

		/// <summary>
		/// Returns wether or not tracking URL changes is disabled
		/// </summary>
		/// <remarks>
		/// appSetting: 'urlTracker:trackingDisabled'
		/// </remarks>
		bool IsTrackingDisabled();

		/// <summary>
		/// Returns wether or not logging for the UrlTracker is enabled
		/// </summary>
		/// <remarks>
		/// appSettings: 'urlTracker:enableLogging' & 'umbracoDebugMode'
		/// </remarks>
		bool LoggingEnabled();

		/// <summary>
		/// Returns the regex patterns for NotFound urls to ignore
		/// </summary>
		Regex[] GetRegexNotFoundUrlsToIgnore();

		/// <summary>
		/// Returns wether or not not found (404) tracking is disabled
		/// </summary>
		/// <remarks>
		/// appSetting: 'urlTracker:notFoundTrackingDisabled'
		/// </remarks>
		bool IsNotFoundTrackingDisabled();

		/// <summary>
		/// Gets a value indicating whether or not to append port numbers to URLs. Default is true.
		/// </summary>
		/// <value>
		///   <c>true</c> if we are to append the port number; otherwise, <c>false</c>.
		/// </value>
		bool AppendPortNumber();

		/// <summary>
		/// Returns wether or not a child node has a domain configured
		/// </summary>
		/// <remarks>
		/// appSetting: 'urlTracker:hasDomainOnChildNode'
		/// </remarks>
		bool HasDomainOnChildNode();

		/// <summary>
		/// Gets a value indicating whether or not forced redirects should be cached for a period of time. Default is false.
		/// Setting this to true will enabled forced redirect updates and additions to propagate to all servers in a multi-server environment
		/// </summary>
		/// <value>
		///   <c>true</c> if we are to cache forced redirects for a period of time; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>
		/// appSetting: 'urlTracker:forcedRedirectCacheTimeoutEnabled'
		/// </remarks>
		bool IsForcedRedirectCacheTimeoutEnabled();

		/// <summary>
		/// Amount of time, in seconds, that the forced redirects will be cached for. Default is 14400 (4 hours).
		/// The default value will be used when the app setting is less than 1.
		/// This setting does nothing unless urlTracker:forcedRedirectCacheTimeoutEnabled is true
		/// </summary>
		/// <remarks>
		/// appSetting: 'urlTracker:forcedRedirectCacheTimeoutSeconds'
		/// </remarks>
		TimeSpan GetForcedRedirectCacheTimeoutSeconds();

		/// <summary>
		/// Returns the UrlTracker UI URL to ignore as referrer
		/// </summary>
		string GetReferrerToIgnore();
	}
}
