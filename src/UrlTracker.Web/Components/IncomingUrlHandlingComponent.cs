using System;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using Umbraco.Core.Composing;
using Umbraco.Web;
using Umbraco.Web.Routing;
using UrlTracker.Web.Events;
using UrlTracker.Web.Events.Models;
using ILogger = UrlTracker.Core.Logging.ILogger;

namespace UrlTracker.Web.Components
{
    /* What's the matter with this complicated stuff?
     * 
     * Well... Turns out it's not possible to call on static events from outside the class that defines it.
     *    That means that any object that subscribes to static events is by definition untestable.
     *    I need my classes to be testable, so I need to isolate logic that uses static events.
     * How? By pasting my own layer of event handling on top of it, but better.
     */

    [ExcludeFromCodeCoverage]
    public class IncomingUrlHandlingComponent
        : IComponent
    {
        private readonly IEventPublisher<IncomingRequestEventArgs> _eventPublisher;
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private static readonly object _lock = new object();
        private static bool _mustRegisterEventHandlers = true;

        public IncomingUrlHandlingComponent(IEventPublisher<IncomingRequestEventArgs> eventPublisher,
                                            ILogger logger,
                                            IHttpContextAccessor httpContextAccessor)
        {
            _eventPublisher = eventPublisher;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public void Initialize()
        {
            UmbracoApplicationBase.ApplicationInit += OnApplicationInit;
        }

        public void Terminate()
        {
            UmbracoApplicationBase.ApplicationInit -= OnApplicationInit;
        }

        private void OnApplicationInit(object sender, EventArgs e)
        {
            if (!(sender is HttpApplication app))
            {
                return;
            }
            OnApplicationInit(app);
        }

        private void OnApplicationInit(HttpApplication app)
        {
            /* This makes no sense!
             * 
             * Using the double if pattern around a lock ensures that the code inside the inner if statement only runs once.
             * This causes a totally unrelated NullReferenceException however
             *    when I also put the registration of 'PostResolveRequestCache' inside the inner if statement.
             * 
             *  - When I remove the lock, it still won't work
             *  - When I remove the assignment to the _mustRegisterEventHandlers, it DOES work, but registration is executed multiple times
             *  - When I move the registration outside of the inner if statement, but inside the lock, it doesn't work
             *  - When I move the registration outside the lock, inside a separate lock, it doesn't work
             *  - When I move the registration outside of the outer if statement, it DOES work
             *  
             *  This is apparently a bug in IIS or ASP.NET
             *  The fix is inspired by this stackoverflow question and by the old code base: https://stackoverflow.com/a/28362497
             */

            if (_mustRegisterEventHandlers)
            {
                lock (_lock)
                {
                    if (_mustRegisterEventHandlers)
                    {
                        UmbracoModule.EndRequest += OnUmbracoEndRequest;

                        _logger.LogRegisteredEventHandlers<IncomingUrlHandlingComponent>();

                        _mustRegisterEventHandlers = !_mustRegisterEventHandlers;
                    }
                }
            }

            lock (_lock)
            {
                app.PostResolveRequestCache -= OnPostResolveRequestCache;
                app.PostResolveRequestCache += OnPostResolveRequestCache;
            }
        }

        private void OnUmbracoEndRequest(object sender, UmbracoRequestEventArgs e)
        {
            OnIncomingRequest(e.HttpContext);
        }

        private void OnPostResolveRequestCache(object sender, EventArgs e)
        {
            // this is most true to how it will work in .NET 5, where the execution happens in the pipeline
            //    and you get the http context passed in the method
            OnIncomingRequest(new HttpContextWrapper(_httpContextAccessor.HttpContext));
        }

        private void OnIncomingRequest(HttpContextBase httpContext)
        {
            _eventPublisher.PublishAsync(this, new IncomingRequestEventArgs(httpContext));
        }
    }
}
