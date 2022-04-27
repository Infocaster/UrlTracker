using System.Threading.Tasks;

namespace UrlTracker.Web.Events
{
    public interface IEventPublisher<TEventArgs>
        : IEventPublisher<object, TEventArgs>
    { }

    public interface IEventPublisher<in TSource, in TEventArgs>
    {
        Task PublishAsync(TSource source, TEventArgs args);
    }
}