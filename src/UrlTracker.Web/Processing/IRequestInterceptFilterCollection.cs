using System.Threading.Tasks;
using UrlTracker.Core.Domain.Models;

namespace UrlTracker.Web.Processing
{
    public interface IRequestInterceptFilterCollection
    {
        ValueTask<bool> EvaluateCandidateAsync(Url url);
    }
}