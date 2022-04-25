using System.Threading.Tasks;
using UrlTracker.Web.Events.Models;

namespace UrlTracker.Web.Processing
{
    public interface IClientErrorFilter
    {
        ValueTask<bool> EvaluateCandidateAsync(UrlTrackerHandled notification);
    }
}
