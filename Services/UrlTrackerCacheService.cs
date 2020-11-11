using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InfoCaster.Umbraco.UrlTracker.Models;
using InfoCaster.Umbraco.UrlTracker.NewRepositories;
using Umbraco.Core.Cache;
using Umbraco.Core.Scoping;

namespace InfoCaster.Umbraco.UrlTracker.Services
{
	public class UrlTrackerCacheService : IUrlTrackerCacheService
	{
		private readonly IAppPolicyCache _runtimeCache;

		public UrlTrackerCacheService(AppCaches appCaches)
		{
			_runtimeCache = appCaches.RuntimeCache;
		}

		public T Get<T>(string key)
		{
			return _runtimeCache.GetCacheItem<T>(key);
		}

		public void Set<T>(string key, T value, TimeSpan? timeout = null)
		{
			_runtimeCache.InsertCacheItem(key, () =>
			{
				return value;
			}, timeout);
		}

		public void Clear(string key)
		{
			_runtimeCache.ClearByKey(key);
		}
	}
}