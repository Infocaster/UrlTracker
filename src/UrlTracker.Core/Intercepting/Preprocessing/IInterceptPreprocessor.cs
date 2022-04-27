using System.Threading.Tasks;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Intercepting.Models;

namespace UrlTracker.Core.Intercepting.Preprocessing
{
    public interface IInterceptPreprocessor
    {
        ValueTask<IInterceptContext> PreprocessUrlAsync(Url url, IInterceptContext context);
    }
}
