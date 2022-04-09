using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
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

        public IntermediateInterceptService(IInterceptPreprocessorCollection preprocessors,
                                            IInterceptorCollection interceptors)
        {
            _preprocessors = preprocessors;
            _interceptors = interceptors;
        }

        public async Task<ICachableIntercept> GetAsync(Url url, IInterceptContext? context = null)
        {
            context = await _preprocessors.PreprocessUrlAsync(url, context);
            return await _interceptors.InterceptAsync(url, context);
        }
    }
}
