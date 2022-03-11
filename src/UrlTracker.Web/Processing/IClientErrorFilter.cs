using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace UrlTracker.Web.Processing
{
    public interface IClientErrorFilter
    {
        ValueTask<bool> EvaluateCandidateAsync(HttpContext httpContext);
    }
}
