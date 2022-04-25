using System.Threading.Tasks;
using UrlTracker.Web.Events.Models;

namespace UrlTracker.Web.Processing
{
    public interface IClientErrorFilterCollection
    {
        ValueTask<bool> EvaluateCandidacyAsync(UrlTrackerHandled notification);
    }
}