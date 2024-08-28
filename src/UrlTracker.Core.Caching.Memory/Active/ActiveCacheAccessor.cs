using System.Collections.Generic;
using System.Linq;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Core.Caching.Memory.Active
{
    /// <summary>
    /// This type is used by the URL Tracker to hold all redirects in-memory.
    /// </summary>
    public interface IActiveCacheAccessor
    {
        IReadOnlyCollection<IClientError> GetNoLongerExists(IEnumerable<string> urlsAndPaths);
        IReadOnlyCollection<IRedirect> GetRedirect(IEnumerable<string> urlsAndPaths);
        void Set(IDictionary<string, List<IRedirect>> cache);
        void Set(IDictionary<string, List<IClientError>> cache);
        IDictionary<string, List<IRedirect>> GetRedirectCache();
    }

    internal class ActiveCacheAccessor : IActiveCacheAccessor
    {
        private IDictionary<string, List<IRedirect>> _redirectDictionary;
        private IDictionary<string, List<IClientError>> _clientErrorDictionary;

        public ActiveCacheAccessor()
        {
            _redirectDictionary = new Dictionary<string, List<IRedirect>>();
            _clientErrorDictionary = new Dictionary<string, List<IClientError>>();
        }

        public IReadOnlyCollection<IRedirect> GetRedirect(IEnumerable<string> urlsAndPaths)
        {
            IEnumerable<IRedirect> result = Enumerable.Empty<IRedirect>();
            foreach (var url in urlsAndPaths)
            {
                if (_redirectDictionary.TryGetValue(url.ToLower(), out var redirects))
                {
                    result = result.Concat(redirects);
                }
            }

            return new List<IRedirect>(result
                .OrderByDescending(r => r.Force)
                .ThenByDescending(r => r.CreateDate));
        }

        public IReadOnlyCollection<IClientError> GetNoLongerExists(IEnumerable<string> urlsAndPaths)
        {
            IEnumerable<IClientError> result = Enumerable.Empty<IClientError>();
            foreach (var url in urlsAndPaths)
            {
                if (_clientErrorDictionary.TryGetValue(url.ToLower(), out var clientErrors))
                {
                    result = result.Concat(clientErrors);
                }
            }
            return result.ToList();
        }

        public void Set(IDictionary<string, List<IRedirect>> cache)
        {
            _redirectDictionary = cache;
        }

        public void Set(IDictionary<string, List<IClientError>> cache)
        {
            _clientErrorDictionary = cache;
        }

        public IDictionary<string, List<IRedirect>> GetRedirectCache()
        {
            return _redirectDictionary;
        }
    }
}
