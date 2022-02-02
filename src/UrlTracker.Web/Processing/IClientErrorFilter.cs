using System.Threading.Tasks;
using System.Web;

namespace UrlTracker.Web.Processing
{
    public interface IClientErrorFilter
    {
        ValueTask<bool> EvaluateCandidateAsync(HttpContextBase httpContext);
    }
}
