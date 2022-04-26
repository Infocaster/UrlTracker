using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services.Implement;
using Umbraco.Web;
using UrlTracker.Core;
using UrlTracker.Core.Abstractions;
using UrlTracker.Core.Configuration;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Core.Models;

namespace UrlTracker.Web.Components
{
    // ToDo: 
    [ExcludeFromCodeCoverage]
    public class ContentChangeHandlingComponent
        : IComponent
    {
        private readonly IUmbracoContextFactoryAbstraction _umbracoContextFactory;
        private readonly IRedirectService _redirectService;
        private readonly IScopeProvider _scopeProvider;
        private readonly IConfiguration<UrlTrackerSettings> _configuration;
        private readonly IContentValueReaderFactory _contentValueReaderFactory;
        private const string _moveRedirectsKey = "ic:MoveRedirects";
        private const string _renameRedirectsKey = "ic:RenameRedirects";

        public ContentChangeHandlingComponent(IUmbracoContextFactoryAbstraction umbracoContextFactory,
                                              IRedirectService redirectService,
                                              IScopeProvider scopeProvider,
                                              IConfiguration<UrlTrackerSettings> configuration,
                                              IContentValueReaderFactory contentValueReaderFactory)
        {
            _umbracoContextFactory = umbracoContextFactory;
            _redirectService = redirectService;
            _scopeProvider = scopeProvider;
            _configuration = configuration;
            _contentValueReaderFactory = contentValueReaderFactory;
        }

        public void Initialize()
        {
            ContentService.Moving += ContentService_Moving;
            ContentService.Moved += ContentService_Moved;
            ContentService.Publishing += ContentService_Publishing;
            ContentService.Published += ContentService_Published;
        }

        public void Terminate()
        {
            ContentService.Moving -= ContentService_Moving;
            ContentService.Moved -= ContentService_Moved;
            ContentService.Publishing -= ContentService_Publishing;
            ContentService.Published -= ContentService_Published;
        }

        private void ContentService_Publishing(Umbraco.Core.Services.IContentService sender, Umbraco.Core.Events.ContentPublishingEventArgs e)
        {
            if (!TrackingEnabled()) return;

            using (var cref = _umbracoContextFactory.EnsureUmbracoContext())
            {
                List<Redirect> redirects = new List<Redirect>();
                foreach (var entity in e.PublishedEntities)
                {
                    // if content is newly created, no published content exists yet at this point,
                    //    so we also don't have to check for redirects
                    var content = cref.GetContentById(entity.Id);
                    if (content is null) continue;

                    var valueReaders = _contentValueReaderFactory.Create(entity, onlyChanged: true);

                    foreach (var valueReader in valueReaders)
                    {
                        if (!content.IsPublished(valueReader.GetCulture())) continue;
                        if (valueReader.GetName().Equals(valueReader.GetName(content)) &&
                            valueReader.GetValue(Constants.Conventions.Content.UrlName) == valueReader.GetValue(content, Constants.Conventions.Content.UrlName)) continue;

                        // this entity has changed, so a new redirect for it and its descendants must be created
                        var root = content.Root();
                        foreach (var item in content.Descendants(valueReader.GetCulture()).Prepend(content))
                        {
                            redirects.Add(CreateRedirect(root, item, valueReader.GetCulture(), "Url has changed"));
                        }
                    }
                }

                e.EventState.Add(_renameRedirectsKey, redirects);
            }
        }

        private void ContentService_Published(Umbraco.Core.Services.IContentService sender, Umbraco.Core.Events.ContentPublishedEventArgs e)
        {
            if (!TrackingEnabled()) return;

            // Check if the key exists: Fixes an error on sorting;
            //    sorting raises the published event without the publishing event.
            if (!e.EventState.ContainsKey(_renameRedirectsKey)) return;

            var redirects = e.EventState[_renameRedirectsKey] as List<Redirect>;
            RegisterRedirects(redirects);
        }

        private void ContentService_Moving(Umbraco.Core.Services.IContentService sender, Umbraco.Core.Events.MoveEventArgs<Umbraco.Core.Models.IContent> e)
        {
            if (!TrackingEnabled()) return;

            // In this event, it's not sure if the move operation will succeed. It might still get cancelled.
            //    Therefore, only calculate the redirects here, but don't actually register them.
            using (var cref = _umbracoContextFactory.EnsureUmbracoContext())
            {
                List<Redirect> redirects = new List<Redirect>();
                foreach (var moveInfo in e.MoveInfoCollection)
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
                        List<string> cultures = GetCulturesFromContent(item);

                        // make sure to only consider cultures for which a change is actually noticable.
                        //    That is: if a node that can be routed moves to a node that also can be routed.
                        foreach (var c in cultures.Where(c => newParent.AncestorsOrSelf().All(i => i.IsPublished(c)) && item.AncestorsOrSelf().All(i => i.IsPublished(c))))
                        {
                            // Notice that the old IPublishedContent item is used here. This may seem questionable,
                            //    but it's acceptable, since only the id of the published content item will be saved.
                            //    I think it would still be better to use the new IPublishedContent, in case we might
                            //    do more with it in the future.
                            redirects.Add(CreateRedirect(newRoot, item, c, "This page or an ancestor was moved"));
                        }
                    }
                }

                e.EventState.Add(_moveRedirectsKey, redirects);
            }
        }

        private void ContentService_Moved(Umbraco.Core.Services.IContentService sender, Umbraco.Core.Events.MoveEventArgs<Umbraco.Core.Models.IContent> e)
        {
            if (!TrackingEnabled()) return;
            if (!e.EventState.ContainsKey(_moveRedirectsKey)) return;

            // At this point we know for sure that the operation has succeeded, so now we can register the redirects
            //    Wrap everything in a scope: if anything fails, everything will be rolled back and we won't be left with partial changes
            var redirects = e.EventState[_moveRedirectsKey] as List<Redirect>;
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

            if (item.ContentType.VariesByCulture())
            {
                cultures.AddRange(from c in item.Cultures.Values
                                  let cultureString = c.Culture
                                  where item.IsPublished(cultureString)
                                  select cultureString);
            }
            else
            {
                cultures.Add(null);
            }

            return cultures;
        }

        private void RegisterRedirects(List<Redirect> redirects)
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                foreach (var redirect in redirects)
                {
                    // this looks hacky, but it's the easiest way to perform async tasks
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

        private IEnumerable<IPublishedContent> DescendantsAndSelfForAllCultures(IPublishedContent content)
        {
            return content.ChildrenForAllCultures.SelectMany(child => DescendantsAndSelfForAllCultures(child)).Prepend(content);
        }
    }
}
