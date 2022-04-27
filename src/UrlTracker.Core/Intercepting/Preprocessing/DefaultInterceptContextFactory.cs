using System.Diagnostics.CodeAnalysis;
using UrlTracker.Core.Intercepting.Models;

namespace UrlTracker.Core.Intercepting.Preprocessing
{
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
