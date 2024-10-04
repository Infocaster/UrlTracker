using System.Diagnostics.CodeAnalysis;
using UrlTracker.Core.Intercepting.Models;

namespace UrlTracker.Core.Intercepting.Preprocessing
{
    public interface IDefaultInterceptContextFactory
    {
        IInterceptContext Create();
    }

    [ExcludeFromCodeCoverage]
    public class DefaultInterceptContextFactory
        : IDefaultInterceptContextFactory
    {
        public IInterceptContext Create()
        {
            return new DefaultInterceptContext();
        }
    }
}
