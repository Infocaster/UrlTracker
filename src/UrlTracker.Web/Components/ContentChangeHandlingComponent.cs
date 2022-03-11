using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Extensions;
using UrlTracker.Core;
using UrlTracker.Core.Abstractions;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Core.Models;

namespace UrlTracker.Web.Components
{
    // ToDo: 
    [ExcludeFromCodeCoverage]
    public class ContentChangeHandlingComponent
    : INotificationHandler<ContentMovingNotification>,
      INotificationHandler<ContentMovedNotification>,
      INotificationHandler<ContentPublishingNotification>,
      INotificationHandler<ContentPublishedNotification>
    {
        private readonly IUmbracoContextFactoryAbstraction _umbracoContextFactory;
        private readonly IRedirectService _redirectService;
        private readonly IScopeProvider _scopeProvider;
        private readonly IOptions<UrlTrackerSettings> _configuration;
        private const string _moveRedirectsKey = "ic:MoveRedirects";
        private const string _renameRedirectsKey = "ic:RenameRedirects";

        public ContentChangeHandlingComponent(IUmbracoContextFactoryAbstraction umbracoContextFactory,
                                              IRedirectService redirectService,
                                              IScopeProvider scopeProvider,
                                              IOptions<UrlTrackerSettings> configuration)
        {
            _umbracoContextFactory = umbracoContextFactory;
            _redirectService = redirectService;
            _scopeProvider = scopeProvider;
            _configuration = configuration;
        }

        void INotificationHandler<ContentPublishingNotification>.Handle(ContentPublishingNotification notification)
        {
            if (!TrackingEnabled()) return;

            using (var cref = _umbracoContextFactory.EnsureUmbracoContext())
            {
                List<Redirect> redirects = new List<Redirect>();
                foreach (var entity in notification.PublishedEntities)
                {
                    // if content is newly created, no published content exists yet at this point,
                    //    so we also don't have to check for redirects
                    var content = cref.GetContentById(entity.Id);
                    if (content is null) continue;

                    var cultures = GetCulturesFromContent(content);

                    foreach (var c in cultures)
                    {
                        if (entity.GetCultureName(c).Equals(content.Name(c)) &&
                            entity.GetValue<string>(Constants.Conventions.Content.UrlName, c) == content.Value<string>(Constants.Conventions.Content.UrlName, c)) continue;

                        // this entity has changed, so a new redirect for it and its descendants must be created
                        var root = content.Root();
                        foreach (var item in content.Descendants(c).Prepend(content))
                        {
                            redirects.Add(CreateRedirect(root, item, c, "Url has changed"));
                        }
                    }
                }

                notification.State.Add(_renameRedirectsKey, redirects);
            }
        }

        void INotificationHandler<ContentPublishedNotification>.Handle(ContentPublishedNotification notification)
        {
            if (!TrackingEnabled()) return;
            if (!notification.State.ContainsKey(_renameRedirectsKey)) return;

            var redirects = notification.State[_renameRedirectsKey] as List<Redirect>;
            RegisterRedirects(redirects);
        }

        void INotificationHandler<ContentMovingNotification>.Handle(ContentMovingNotification notification)
        {
            if (!TrackingEnabled()) return;

            // In this event, it's not sure if the move operation will succeed. It might still get cancelled.
            //    Therefore, only calculate the redirects here, but don't actually register them.
            using (var cref = _umbracoContextFactory.EnsureUmbracoContext())
            {
                List<Redirect> redirects = new List<Redirect>();
                foreach (var moveInfo in notification.MoveInfoCollection)
                {
                    var content = cref.GetContentById(moveInfo.Entity.Id);
                    var newRoot = cref.GetContentById(moveInfo.NewParentId).Root();
                    foreach (var item in DescendantsForAllCultures(content).Prepend(content))
                    {
                        List<string> cultures = GetCulturesFromContent(item);

                        foreach (var c in cultures)
                        {
                            // Notice that the old IPublishedContent item is used here. This may seem questionable,
                            //    but it's acceptable, since only the id of the published content item will be saved.
                            //    I think it would still be better to use the new IPublishedContent, in case we might
                            //    do more with it in the future.
                            redirects.Add(CreateRedirect(newRoot, item, c, "This page was moved"));
                        }
                    }
                }

                notification.State.Add(_moveRedirectsKey, redirects);
            }
        }

        void INotificationHandler<ContentMovedNotification>.Handle(ContentMovedNotification notification)
        {
            if (!TrackingEnabled()) return;
            if (!notification.State.ContainsKey(_moveRedirectsKey)) return;

            // At this point we know for sure that the operation has succeeded, so now we can register the redirects
            //    Wrap everything in a scope: if anything fails, everything will be rolled back and we won't be left with partial changes
            var redirects = notification.State[_moveRedirectsKey] as List<Redirect>;
            RegisterRedirects(redirects);
        }

        private static Redirect CreateRedirect(IPublishedContent root, IPublishedContent item, string culture, string notes)
        {
            return new Redirect
            {
                Culture = culture,
                Force = false,
                Notes = notes,
                PassThroughQueryString = true,
                SourceRegex = null,
                SourceUrl = item.Url(culture, UrlMode.Absolute),
                TargetNode = item,
                TargetRootNode = root,
                TargetStatusCode = HttpStatusCode.MovedPermanently,
                TargetUrl = null
            };
        }

        private static List<string> GetCulturesFromContent(IPublishedContent item)
        {
            List<string> cultures = new List<string>();
            if (item.Cultures.Any(c => !string.IsNullOrWhiteSpace(c.Value.Culture))) cultures.AddRange(from c in item.Cultures.Values select c.Culture);
            else cultures.Add(null);
            return cultures;
        }

        private void RegisterRedirects(List<Redirect> redirects)
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                foreach (var redirect in redirects)
                {
                    // this looks hacky, but it's the only way to perform async tasks
                    //    in a sync context without potentially creating deadlocks
                    Task.Run(() => _redirectService.AddAsync(redirect).Wait()).Wait();
                }

                scope.Complete();
            }
        }

        private bool TrackingEnabled()
        {
            var configurationValue = _configuration.Value;
            return !(configurationValue.IsDisabled || configurationValue.IsTrackingDisabled);
        }

        private IEnumerable<IPublishedContent> DescendantsForAllCultures(IPublishedContent content)
        {
            return content.ChildrenForAllCultures.SelectMany(child => DescendantsForAllCultures(child));
        }
    }
}
