using System;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using umbraco;

namespace InfoCaster.Umbraco.UrlTracker.Settings
{
	public class UrlTrackerSettings : IUrlTrackerSettings
	{
		private string TableName => "icUrlTracker";
		private string OldTableName => "infocaster301";
		private string HttpModuleCheck => "890B748D-E226-49FA-A0D7-9AFD3A359F88";

		#region Lazy initialization

		private Lazy<bool> _seoMetadataInstalled => new Lazy<bool>(() =>
		{
			return AppDomain.CurrentDomain.GetAssemblies().Any(x => x.FullName.Contains("Epiphany.SeoMetadata"));
		});

		private Lazy<string> _seoMetadataPropertyName => new Lazy<string>(() =>
		{
			return !string.IsNullOrEmpty(ConfigurationManager.AppSettings["SeoMetadata.PropertyName"]) ? ConfigurationManager.AppSettings["SeoMetadata.PropertyName"] : "metadata";
		});

		private Lazy<bool> _isDisabled => new Lazy<bool>(() =>
		{
			if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["urlTracker:disabled"]))
			{
				bool parsedAppSetting;
				if (bool.TryParse(ConfigurationManager.AppSettings["urlTracker:disabled"], out parsedAppSetting)) { }
				return parsedAppSetting;
			}
			return false;
		});

		private Lazy<bool> _enableLogging => new Lazy<bool>(() =>
		{
			if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["urlTracker:enableLogging"]))
			{
				bool parsedAppSetting;
				if (bool.TryParse(ConfigurationManager.AppSettings["urlTracker:enableLogging"], out parsedAppSetting))
					return parsedAppSetting;
			}
			return false;
		});

		private Lazy<Regex[]> _regexNotFoundUrlsToIgnore => new Lazy<Regex[]>(() =>
		{
			return new[] { new Regex("__browserLink/requestData/.*", RegexOptions.Compiled), new Regex("[^/]/arterySignalR/ping", RegexOptions.Compiled) };
		});

		private Lazy<bool> _isTrackingDisabled => new Lazy<bool>(() =>
		{
			if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["urlTracker:trackingDisabled"]))
			{
				bool parsedAppSetting;
				if (bool.TryParse(ConfigurationManager.AppSettings["urlTracker:trackingDisabled"], out parsedAppSetting))
					return parsedAppSetting;
			}
			return false;
		});

		private Lazy<bool> _isNotFoundTrackingDisabled => new Lazy<bool>(() =>
		{
			if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["urlTracker:notFoundTrackingDisabled"]))
			{
				bool parsedAppSetting;
				if (bool.TryParse(ConfigurationManager.AppSettings["urlTracker:notFoundTrackingDisabled"], out parsedAppSetting))
					return parsedAppSetting;
			}
			return false;
		});

		private Lazy<bool> _appendPortNumber => new Lazy<bool>(() =>
		{
			if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["urlTracker:appendPortNumber"]))
			{
				if (bool.TryParse(ConfigurationManager.AppSettings["urlTracker:appendPortNumber"], out bool parsedAppSetting))
					return parsedAppSetting;
			}

			return true;
		});

		private Lazy<bool> _hasDomainOnChildNode => new Lazy<bool>(() =>
		{
			if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["urlTracker:hasDomainOnChildNode"]))
			{
				bool parsedAppSetting;
				if (bool.TryParse(ConfigurationManager.AppSettings["urlTracker:hasDomainOnChildNode"], out parsedAppSetting))
					return parsedAppSetting;
			}
			return false;
		});

		private Lazy<bool> _forcedRedirectCacheTimeoutEnabled => new Lazy<bool>(() =>
		{
			if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["urlTracker:forcedRedirectCacheTimeoutEnabled"]))
			{
				bool parsedAppSetting;
				if (bool.TryParse(ConfigurationManager.AppSettings["urlTracker:forcedRedirectCacheTimeoutEnabled"], out parsedAppSetting))
					return parsedAppSetting;
			}

			return false;
		});

		private Lazy<TimeSpan> _forcedRedirectCacheTimeoutSeconds => new Lazy<TimeSpan>(() =>
		{
			if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["urlTracker:forcedRedirectCacheTimeoutSeconds"]))
			{
				int parsedAppSetting;

				if (int.TryParse(ConfigurationManager.AppSettings["urlTracker:forcedRedirectCacheTimeoutSeconds"], out parsedAppSetting) && parsedAppSetting > 0)
					return new TimeSpan(0, 0, parsedAppSetting);
			}
			return new TimeSpan(0, 0, 14400); // 4 hours
		});

		#endregion

		public string GetTableName()
		{
			return TableName;
		}

		public string GetOldTableName()
		{
			return OldTableName;
		}

		public string GetHttpModuleCheck()
		{
			return HttpModuleCheck;
		}

		public bool IsSEOMetadataInstalled()
		{
			return _seoMetadataInstalled.Value;
		}

		public string GetSEOMetadataPropertyName()
		{
			return _seoMetadataPropertyName.Value;
		}

		public bool IsDisabled()
		{
			return _isDisabled.Value;
		}

		public bool IsTrackingDisabled()
		{
			return _isTrackingDisabled.Value;
		}

		public bool LoggingEnabled()
		{
			return _enableLogging.Value;
		}

		public Regex[] GetRegexNotFoundUrlsToIgnore()
		{
			return _regexNotFoundUrlsToIgnore.Value;
		}

		public bool IsNotFoundTrackingDisabled()
		{
			return _isNotFoundTrackingDisabled.Value;
		}

		public bool AppendPortNumber()
		{
			return _appendPortNumber.Value;
		}

		public bool HasDomainOnChildNode()
		{
			return _hasDomainOnChildNode.Value;
		}

		public bool IsForcedRedirectCacheTimeoutEnabled()
		{
			return _forcedRedirectCacheTimeoutEnabled.Value;
		}

		public TimeSpan GetForcedRedirectCacheTimeoutSeconds()
		{
			return _forcedRedirectCacheTimeoutSeconds.Value;
		}

		public string GetReferrerToIgnore()
		{
			return "Umbraco/UrlTracker/InfoCaster.Umbraco.UrlTracker.UI";
		}
	}
}