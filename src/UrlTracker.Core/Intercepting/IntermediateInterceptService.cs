using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Scoping;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Core.Intercepting.Preprocessing;

namespace UrlTracker.Core.Intercepting
{
    [ExcludeFromCodeCoverage]
    public class IntermediateInterceptService
        : IIntermediateInterceptService
    {
        private readonly IInterceptPreprocessorCollection _preprocessors;
        private readonly IInterceptorCollection _interceptors;
        private readonly IScopeProvider _scopeProvider;

        public IntermediateInterceptService(IInterceptPreprocessorCollection preprocessors,
                                            IInterceptorCollection interceptors,
                                            IScopeProvider scopeProvider)
        {
            _preprocessors = preprocessors;
            _interceptors = interceptors;
            _scopeProvider = scopeProvider;
        }

        public async Task<ICachableIntercept> GetAsync(Url url, IInterceptContext? context = null)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);

            context = await _preprocessors.PreprocessUrlAsync(url, context);
            return await _interceptors.InterceptAsync(url, context);
        }
    }
}
