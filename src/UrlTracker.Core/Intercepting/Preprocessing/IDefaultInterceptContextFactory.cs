using UrlTracker.Core.Intercepting.Models;

namespace UrlTracker.Core.Intercepting.Preprocessing
{
    public interface IDefaultInterceptContextFactory
    {
        IInterceptContext Create();
    }
}