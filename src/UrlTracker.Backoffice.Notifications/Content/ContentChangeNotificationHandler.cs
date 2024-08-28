using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;
using UrlTracker.Backoffice.Notifications.Options;
using UrlTracker.Core;
using UrlTracker.Core.Abstractions;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Core.Models;

namespace UrlTracker.Backoffice.Notifications.Content
{
    // ToDo: Make this logic testable
    /// <summary>
    /// A notification handler that creates redirects based on changes in content
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ContentChangeNotificationHandler
    : INotificationAsyncHandler<ContentMovingNotification>,
      INotificationAsyncHandler<ContentMovedNotification>,
      INotificationAsyncHandler<ContentPublishingNotification>,
      INotificationAsyncHandler<ContentPublishedNotification>
    {
        private readonly IUmbracoContextFactoryAbstraction _umbracoContextFactory;
        private readonly IRedirectService _redirectService;
        private readonly IScopeProvider _scopeProvider;
        private readonly IOptionsMonitor<UrlTrackerSettings> _configuration;
        private readonly IOptionsMonitor<UrlTrackerNotificationsOptions> _notificationsOptions;
        private readonly IContentValueReaderFactory _contentValueReaderFactory;
        private readonly IPublishedUrlProvider _publishedUrlProvider;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly ILogger<ContentChangeNotificationHandler> _logger;
        private const string _moveRedirectsKey = "ic:MoveRedirects";
        private const string _renameRedirectsKey = "ic:RenameRedirects";

        /// <inheritdoc />
        public ContentChangeNotificationHandler(IUmbracoContextFactoryAbstraction umbracoContextFactory,
                                                IRedirectService redirectService,
                                                IScopeProvider scopeProvider,
                                                IOptionsMonitor<UrlTrackerSettings> configuration,
                                                IOptionsMonitor<UrlTrackerNotificationsOptions> notificationsOptions,
                                                IContentValueReaderFactory contentValueReaderFactory,
                                                IPublishedUrlProvider publishedUrlProvider,
                                                IVariationContextAccessor variationContextAccessor,
                                                ILogger<ContentChangeNotificationHandler> logger)
        {
            _umbracoContextFactory = umbracoContextFactory;
            _redirectService = redirectService;
            _scopeProvider = scopeProvider;
            _configuration = configuration;
            _notificationsOptions = notificationsOptions;
            _contentValueReaderFactory = contentValueReaderFactory;
            _publishedUrlProvider = publishedUrlProvider;
            _variationContextAccessor = variationContextAccessor;
            _logger = logger;
        }

        /* For now the approach is to create working code.
         *     Event management needs to be fleshed out a lot better though.
         *     TODO: identify all different cases for each event and what action should be taken by the url tracker.
         */
        /// <inheritdoc/>
        public Task HandleAsync(ContentPublishingNotification notification, CancellationToken cancellationToken)
        {
            if (!TrackingEnabled())
                return Task.CompletedTask;

            try
            {
                using var cref = _umbracoContextFactory.EnsureUmbracoContext();
                List<Redirect> redirects = new();
                foreach (var entity in notification.PublishedEntities)
                {
                    // if content is newly created, no published content exists yet at this point,
                    //    so we also don't have to check for redirects
                    var content = cref.GetContentById(entity.Id);
                    if (content is null) continue;

                    var valueReaders = _contentValueReaderFactory.Create(entity, onlyChanged: true);

                    foreach (var valueReader in valueReaders)
                    {
                        if (!content.IsPublished(valueReader.GetCulture())) continue;
                        if (valueReader.GetName() == valueReader.GetName(content) &&
                            valueReader.GetValue(Constants.Conventions.Content.UrlName) == valueReader.GetValue(content, Constants.Conventions.Content.UrlName)) continue;

                        // this entity has changed, so a new redirect for it and its descendants must be created
                        var root = content.Root()!;
                        foreach (var item in content.Descendants(_variationContextAccessor, valueReader.GetCulture()).Prepend(content))
                        {
                            redirects.Add(CreateRedirect(item, valueReader.GetCulture()));
                        }
                    }
                }

                notification.State.Add(_renameRedirectsKey, redirects);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "URL Tracker failed to create automatic redirects");
                notification.Messages.Add(new EventMessage("URL Tracker", "An error occurred while attempting to automatically redirect an old URL. Check the logs for more information", EventMessageType.Warning));
            }
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task HandleAsync(ContentPublishedNotification notification, CancellationToken cancellationToken)
        {
            if (!TrackingEnabled()) return;
            if (!notification.State.ContainsKey(_renameRedirectsKey)) return;

            try
            {
                var redirects = (List<Redirect>)notification.State[_renameRedirectsKey]!;
                await RegisterRedirectsAsync(redirects);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "URL Tracker failed to create automatic redirects");
                notification.Messages.Add(new EventMessage("URL Tracker", "An error occurred while attempting to automatically redirect an old URL. Check the logs for more information", EventMessageType.Warning));
            }
        }

        /// <inheritdoc/>
        public Task HandleAsync(ContentMovingNotification notification, CancellationToken cancellationToken)
        {
            if (!TrackingEnabled())
                return Task.CompletedTask;

            try
            {
                // In this event, it's not sure if the move operation will succeed. It might still get cancelled.
                //    Therefore, only calculate the redirects here, but don't actually register them.
                using var cref = _umbracoContextFactory.EnsureUmbracoContext();
                List<Redirect> redirects = new();
                foreach (var moveInfo in notification.MoveInfoCollection)
                {
                    // Content may not have been published yet. If it's not published, then it's not necessary to register redirects
                    var content = cref.GetContentById(moveInfo.Entity.Id);
                    if (content is null) continue;

                    // Parent or root might also not be published yet.
                    var newParent = cref.GetContentById(moveInfo.NewParentId);
                    if (newParent is null) continue;

                    var newRoot = newParent.Root();
                    if (newRoot is null) continue;

                    foreach (var item in DescendantsAndSelfForAllCultures(content))
                    {
                        List<string?> cultures = GetCulturesFromContent(item);

                        // make sure to only consider cultures for which a change is actually noticable.
                        //    That is: if a node that can be routed moves to a node that also can be routed.
                        foreach (var c in cultures.Where(c => newParent.AncestorsOrSelf().All(i => i.IsPublished(c)) && item.AncestorsOrSelf().All(i => i.IsPublished(c))))
                        {
                            // Notice that the old IPublishedContent item is used here. This may seem questionable,
                            //    but it's acceptable, since only the id of the published content item will be saved.
                            //    I think it would still be better to use the new IPublishedContent, in case we might
                            //    do more with it in the future.
                            redirects.Add(CreateRedirect(item, c));
                        }
                    }
                }

                notification.State.Add(_moveRedirectsKey, redirects);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "URL Tracker failed to create automatic redirects");
                notification.Messages.Add(new EventMessage("URL Tracker", "An error occurred while attempting to automatically redirect an old URL. Check the logs for more information", EventMessageType.Warning));
            }
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task HandleAsync(ContentMovedNotification notification, CancellationToken cancellationToken)
        {
            if (!TrackingEnabled()) return;
            if (!notification.State.ContainsKey(_moveRedirectsKey)) return;

            // An error on the URL Tracker should not prevent a user from making a change, therefore the exception is caught and logged and the backoffice user receives a more friendly notification.
            try
            {
                // At this point we know for sure that the operation has succeeded, so now we can register the redirects
                //    Wrap everything in a scope: if anything fails, everything will be rolled back and we won't be left with partial changes
                var redirects = (List<Redirect>)notification.State[_moveRedirectsKey]!;
                await RegisterRedirectsAsync(redirects);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "URL Tracker failed to create automatic redirects");
                notification.Messages.Add(new EventMessage("URL Tracker", "An error occurred while attempting to automatically redirect an old URL. Check the logs for more information", EventMessageType.Warning));
            }
        }

        private Redirect CreateRedirect(IPublishedContent item, string? culture)
        {
            return new Redirect
            {
                Force = false,
                RetainQuery = true,
                Source = new UrlSourceStrategy(item.Url(_publishedUrlProvider, culture, UrlMode.Absolute)),
                Target = new ContentPageTargetStrategy(item, culture),
                Permanent = true
            };
        }

        private static List<string?> GetCulturesFromContent(IPublishedContent item)
        {
            List<string?> cultures = new();

            if (item.ContentType.VariesByCulture())
            {
                cultures.AddRange(from c in item.Cultures.Values
                                  let cultureString = c.Culture.NormalizeCulture()
                                  where item.IsPublished(cultureString)
                                  select cultureString);
            }
            else
            {
                cultures.Add(null);
            }

            return cultures;
        }

        private async Task RegisterRedirectsAsync(List<Redirect> redirects)
        {
            using var scope = _scopeProvider.CreateScope();
            foreach (var redirect in redirects)
            {
                // this looks hacky, but it's the only way to perform async tasks
                //    in a sync context without potentially creating deadlocks
                await _redirectService.AddAsync(redirect);
            }

            scope.Complete();
        }

        private bool TrackingEnabled()
        {
            var configurationValue = _configuration.CurrentValue;
            var notifcationsOptionsValue = _notificationsOptions.CurrentValue;
            return configurationValue.Enable && notifcationsOptionsValue.Enable;
        }

        private static IEnumerable<IPublishedContent> DescendantsAndSelfForAllCultures(IPublishedContent content)
        {
            return content.AsEnumerableOfOne().Concat(content.ChildrenForAllCultures?.SelectMany(child => DescendantsAndSelfForAllCultures(child)) ?? Enumerable.Empty<IPublishedContent>());
        }
    }
}
