using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace UrlTracker.Web.Processing
{
    public interface IClientErrorFilterCollection
    {
        ValueTask<bool> EvaluateCandidacyAsync(HttpContext httpContext);
    }
}