using System;

namespace InfoCaster.Umbraco.UrlTracker.Services
{
	public interface IUrlTrackerCacheService
	{
		T Get<T>(string key);
		void Set<T>(string key, T value, TimeSpan? timeout = null);
		void Clear(string key);
	}
}