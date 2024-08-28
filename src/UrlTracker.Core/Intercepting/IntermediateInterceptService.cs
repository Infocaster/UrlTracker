using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Umbraco.Cms.Infrastructure.Scoping;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Core.Intercepting.Preprocessing;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Intercepting
{
    [ExcludeFromCodeCoverage]
    public class IntermediateInterceptService
        : IIntermediateInterceptService
    {
        private readonly IInterceptorCollection _interceptors;
        private readonly IScopeProvider _scopeProvider;
        private readonly IDefaultInterceptContextFactory _defaultInterceptContextFactory;

        public IntermediateInterceptService(IInterceptorCollection interceptors,
                                            IScopeProvider scopeProvider,
                                            IDefaultInterceptContextFactory defaultInterceptContextFactory)
        {
            _interceptors = interceptors;
            _scopeProvider = scopeProvider;
            _defaultInterceptContextFactory = defaultInterceptContextFactory;
        }

        public async Task<ICachableIntercept> GetAsync(Url url, IInterceptContext? context = null)
        {
            using var scope = _scopeProvider.CreateCoreScope(autoComplete: true);

            context ??= _defaultInterceptContextFactory.Create();
            return await _interceptors.InterceptAsync(url, context);
        }
    }
}
