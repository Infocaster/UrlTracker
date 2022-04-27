using System.Threading.Tasks;
using UrlTracker.Core.Caching;
using UrlTracker.Core.Database.Models;
using UrlTracker.Core.Models;

namespace UrlTracker.Core
{
    public class DecoratorRedirectServiceCaching : IRedirectService
    {
        private readonly IRedirectService _decoratee;
        private readonly IInterceptCache _interceptCache;

        public DecoratorRedirectServiceCaching(IRedirectService decoratee, IInterceptCache interceptCache)
        {
            _decoratee = decoratee;
            _interceptCache = interceptCache;
        }

        public async Task<Redirect> AddAsync(Redirect redirect)
        {
            var result = await _decoratee.AddAsync(redirect);
            _interceptCache.Clear();
            return result;
        }

        public Task<RedirectCollection> GetAsync(uint skip, uint take, string query = null, OrderBy order = OrderBy.Created, bool descending = true)
        {
            return _decoratee.GetAsync(skip, take, query, order, descending);
        }

        public Task<RedirectCollection> GetAsync()
        {
            return _decoratee.GetAsync();
        }

        public async Task<Redirect> UpdateAsync(Redirect redirect)
        {
            var result = await _decoratee.UpdateAsync(redirect);
            _interceptCache.Clear();
            return result;
        }
    }
}
