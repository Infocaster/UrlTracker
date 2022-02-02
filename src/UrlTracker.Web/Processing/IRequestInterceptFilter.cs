using System.Threading.Tasks;
using UrlTracker.Core.Domain.Models;

namespace UrlTracker.Web.Processing
{
    public interface IRequestInterceptFilter
    {
        ValueTask<bool> EvaluateCandidateAsync(Url url);
    }
}
