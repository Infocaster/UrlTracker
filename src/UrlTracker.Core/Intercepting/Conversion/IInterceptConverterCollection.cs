using System.Threading.Tasks;
using UrlTracker.Core.Intercepting.Models;

namespace UrlTracker.Core.Intercepting.Conversion
{
    public interface IInterceptConverterCollection
    {
        ValueTask<IIntercept> ConvertAsync(ICachableIntercept cachableIntercept);
    }
}