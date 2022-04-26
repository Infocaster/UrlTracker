using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Web;
using UrlTracker.Core.Intercepting.Models;

namespace UrlTracker.Web.Processing
{
    [ExcludeFromCodeCoverage]
    public abstract class ResponseInterceptHandlerBase<TInput>
        : ISpecificResponseInterceptHandler
    {
        public virtual bool CanHandle(IIntercept intercept)
            => intercept.Info is TInput;

        public ValueTask HandleAsync(HttpContextBase context, IIntercept intercept)
            => intercept.Info is TInput realIntercept ? HandleAsync(context, realIntercept) : new ValueTask();

        protected abstract ValueTask HandleAsync(HttpContextBase context, TInput intercept);
    }
}
