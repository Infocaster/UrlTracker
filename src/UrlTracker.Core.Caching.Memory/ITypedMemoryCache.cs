using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace UrlTracker.Core.Caching.Memory
{
    /// <summary>
    /// When implemented, this type provides in-memory caching capabilities for objects of one specific type
    /// </summary>
    /// <typeparam name="TKey">The type to use as key</typeparam>
    /// <typeparam name="TValue">The type of objects to cache</typeparam>
    public interface ITypedMemoryCache<TKey, TValue>
        : IDisposable
        where TKey : notnull
    {
        /// <summary>
        /// When implemented, this method fetches an object from in-memory cache or creates a new instance if no object is cached with the given key
        /// </summary>
        /// <param name="key">The unique identifier for a cached object</param>
        /// <param name="factory">A factory function that creates a new instance of <typeparamref name="TValue"/></param>
        /// <param name="options">Caching rules to apply if a new instance were to be created</param>
        /// <returns>An instance of <typeparamref name="TValue"/> from the in-memory cache</returns>
        Task<TValue> GetOrCreateAsync(TKey key, Func<Task<TValue>> factory, MemoryCacheEntryOptions? options = null);

        /// <summary>
        /// When implemented, this method removes all objects from in-memory cache
        /// </summary>
        void Clear();
    }
}