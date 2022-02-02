using System.Threading.Tasks;
using System.Web;

namespace UrlTracker.Web.Processing
{
    public interface IClientErrorFilterCollection
    {
        ValueTask<bool> EvaluateCandidacyAsync(HttpContextBase httpContext);
    }
}