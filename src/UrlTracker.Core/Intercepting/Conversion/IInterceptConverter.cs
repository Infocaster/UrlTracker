using System.Threading.Tasks;
using UrlTracker.Core.Intercepting.Models;

namespace UrlTracker.Core.Intercepting.Conversion
{
    public interface IInterceptConverter
    {
        ValueTask<IIntercept> ConvertAsync(ICachableIntercept cachableIntercept);
    }
}
