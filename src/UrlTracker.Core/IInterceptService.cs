using System.Threading.Tasks;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Intercepting.Models;

namespace UrlTracker.Core
{
    public interface IInterceptService
    {
        Task<IIntercept> GetAsync(Url url);
    }
}