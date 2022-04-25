using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace UrlTracker.Core.Caching
{
    public class TypedMemoryCache<TKey, TValue>
        : ITypedMemoryCache<TKey, TValue>
        where TKey : notnull
    {
        private readonly IMemoryCache _cache;
        private CancellationTokenSource _clearTokenSource;
        private readonly object _lock = new();
        private bool _disposedValue;

        public TypedMemoryCache(long sizeLimit)
        {
            _cache = new MemoryCache(Options.Create(new MemoryCacheOptions
            {
                SizeLimit = sizeLimit
            }));
            _clearTokenSource = new CancellationTokenSource();
        }

        // Implementation inspired by stack overflow: https://stackoverflow.com/a/65683029/2853950
        [ExcludeFromCodeCoverage]
        public Task<TValue> GetOrCreateAsync(TKey key, Func<Task<TValue>> factory, MemoryCacheEntryOptions? options = null)
        {
            if (!_cache.TryGetValue(key, out Task<TValue> task))
            {
                // create a new entry if it doesn't exist yet
                var entry = _cache.CreateEntry(key);
                options ??= new MemoryCacheEntryOptions();

                // add the cleartoken source as an expiration token. This entry will expire when the cache is cleared.
                options.AddExpirationToken(new CancellationChangeToken(_clearTokenSource.Token));
                options.Size ??= 1;
                entry.SetOptions(options);

                // a cancellation token will help ensure that no faulty or double entries make it into the cache.
                var cts = new CancellationTokenSource();
                var newTaskTask = new Task<Task<TValue>>(() => ExecuteFactory(factory, cts));
                var newTask = newTaskTask.Unwrap();
                entry.ExpirationTokens.Add(new CancellationChangeToken(cts.Token));
                entry.Value = newTask;

                // The Dispose actually inserts the entry in the cache
                entry.Dispose();

                // We can now get the cached task from the cache again.
                //    If it's not in the cache, it has been removed from the cache by a different thread
                //    If it is in the cache, then check if it's the same as the one we just created.
                //      If it's the same, we can execute the task and get the result
                //      If it's not the same, then another thread has already created and executed the task. We can take the result from that one instead.
                if (!_cache.TryGetValue(key, out task)) task = newTask;
                if (task == newTask)
                    newTaskTask.RunSynchronously(TaskScheduler.Default);
                else
                    cts.Dispose();
            }
            return task;
        }

        [ExcludeFromCodeCoverage]
        private static async Task<TValue> ExecuteFactory(Func<Task<TValue>> factory, CancellationTokenSource cts)
        {
            try { return await factory().ConfigureAwait(false); }
            catch { cts.Cancel(); throw; }
            finally { cts.Dispose(); }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _clearTokenSource.Cancel();
                _clearTokenSource.Dispose();
                _clearTokenSource = new CancellationTokenSource();
            }
        }

        #region IDisposable implementation
        [ExcludeFromCodeCoverage]
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _cache.Dispose();
                    _clearTokenSource.Dispose();
                }

                _disposedValue = true;
            }
        }

        [ExcludeFromCodeCoverage]
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
