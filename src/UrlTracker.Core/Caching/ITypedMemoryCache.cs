using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace UrlTracker.Core.Caching
{
    public interface ITypedMemoryCache<TKey, TValue>
        : IDisposable
    {
        Task<TValue> GetOrCreateAsync(TKey key, Func<Task<TValue>> factory, MemoryCacheEntryOptions options = null);
        void Clear();
    }
}