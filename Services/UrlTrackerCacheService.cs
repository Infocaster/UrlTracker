using System;
using Umbraco.Core.Cache;

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