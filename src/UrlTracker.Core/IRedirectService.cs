using System.Threading.Tasks;
using UrlTracker.Core.Database.Models;
using UrlTracker.Core.Models;

namespace UrlTracker.Core
{
    public interface IRedirectService
    {
        Task<Redirect> AddAsync(Redirect redirect);
        Task DeleteAsync(Redirect redirect);
        Task<RedirectCollection> GetAsync(uint skip, uint take, string query = null, OrderBy order = OrderBy.Created, bool descending = true);
        Task<RedirectCollection> GetAsync();
        Task<Redirect> GetAsync(int id);
        Task<Redirect> UpdateAsync(Redirect redirect);
    }
}