using System.Threading.Tasks;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Intercepting.Models;

namespace UrlTracker.Core.Intercepting
{
    public interface IIntermediateInterceptService
    {
        Task<ICachableIntercept?> GetAsync(Url url, IInterceptContext? context = null);
    }
}