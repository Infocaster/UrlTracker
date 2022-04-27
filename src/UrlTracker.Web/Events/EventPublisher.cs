using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using ILogger = UrlTracker.Core.Logging.ILogger;

namespace UrlTracker.Web.Events
{
    [ExcludeFromCodeCoverage]
    public class EventPublisher<TEventArgs>
        : EventPublisher<object, TEventArgs>, IEventPublisher<TEventArgs>
    {
        public EventPublisher(IEnumerable<IEventSubscriber<object, TEventArgs>> subscribers, ILogger logger)
            : base(subscribers, logger)
        { }
    }

    [ExcludeFromCodeCoverage]
    public class EventPublisher<TSource, TEventArgs>
        : IEventPublisher<TSource, TEventArgs>
    {
        private readonly IEnumerable<IEventSubscriber<TSource, TEventArgs>> _subscribers;
        private readonly ILogger _logger;

        public EventPublisher(IEnumerable<IEventSubscriber<TSource, TEventArgs>> subscribers, ILogger logger)
        {
            _subscribers = subscribers;
            _logger = logger;
        }

        public async Task PublishAsync(TSource source, TEventArgs args)
        {
            _logger.LogEventPublished<EventPublisher<TSource, TEventArgs>>(args.GetType(), source.GetType());
            foreach (var subscriber in _subscribers)
            {
                try
                {
                    await subscriber.HandleAsync(source, args);
                }
                catch (Exception e)
                {
                    _logger.LogSubscriberError<EventPublisher<TSource, TEventArgs>>(e, subscriber.GetType(), typeof(TEventArgs));
                }
            }
        }
    }
}
