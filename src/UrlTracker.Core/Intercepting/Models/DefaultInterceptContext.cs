using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace UrlTracker.Core.Intercepting.Models
{
    [ExcludeFromCodeCoverage]
    public class DefaultInterceptContext
        : IInterceptContext
    {
        private readonly Dictionary<string, object> _features;

        public int Count => _features.Count;

        public IReadOnlyCollection<string> Keys => _features.Keys;

        public DefaultInterceptContext()
        {
            _features = new Dictionary<string, object>();
        }

        public void Set(string key, object value)
            => _features[key] = value;

        public T Get<T>(string key)
            => _features.TryGetValue(key, out var result) ? (T)result : default;
    }
}
