using System.Threading.Tasks;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Intercepting
{
    public interface IIntermediateInterceptService
    {
        Task<ICachableIntercept> GetAsync(Url url, IInterceptContext? context = null);
    }
}