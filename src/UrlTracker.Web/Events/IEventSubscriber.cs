using System.Threading.Tasks;

namespace UrlTracker.Web.Events
{
    public interface IEventSubscriber<in TSource, in TEventArgs>
    {
        Task HandleAsync(TSource source, TEventArgs args);
    }
}
