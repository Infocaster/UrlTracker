using InfoCaster.Umbraco.UrlTracker.Extensions;
using InfoCaster.Umbraco.UrlTracker.Helpers;
using InfoCaster.Umbraco.UrlTracker.Models;
using InfoCaster.Umbraco.UrlTracker.Repositories;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
//using umbraco;
//using umbraco.BusinessLogic;
//using umbraco.cms.businesslogic;
//using umbraco.cms.businesslogic.web;
//using umbraco.DataLayer;
//using umbraco.NodeFactory;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Implement;
using Umbraco.Web;
using Umbraco.Web.UI;
using umbraco.BasePages;
using Umbraco.Core.Models.PublishedContent;
using Microsoft.Owin.Security.Provider;
using System.Web.UI.WebControls;

using InfoCaster.Umbraco.UrlTracker;
using InfoCaster.Umbraco.UrlTracker.Modules;
using InfoCaster.Umbraco.UrlTracker.Providers;
using System.Web.Hosting;

namespace InfoCaster.Umbraco.UrlTracker
{

    [RuntimeLevel(MinLevel = RuntimeLevel.Boot)]
    public class UrlTrackerComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Components().Append<UrlTrackerComponent>();
        }
    }

    public class UrlTrackerComponent : IComponent
    {
        private readonly UmbracoHelper _umbracoHelper;
        public UrlTrackerComponent(UmbracoHelper umbracoHelper)
        {
            _umbracoHelper = umbracoHelper;
        }

        protected ClientTools ClientTools
        {
            get
            {
                Page page = HttpContext.Current.CurrentHandler as Page;
                if (page != null)
                    return new ClientTools(page);
                return null;
            }
        }

        public void Initialize()
        {
            if (!UrlTrackerSettings.IsDisabled && !UrlTrackerSettings.IsTrackingDisabled)
            {
                UrlTrackerRepository.ReloadForcedRedirectsCache();
                
                ContentService.Moving += ContentService_Moving;
                ContentService.Publishing += ContentService_Publishing;
                ContentService.Published += ContentService_Published;
                ContentService.Deleting += ContentService_Deleting;
                
                //content.BeforeClearDocumentCache += content_BeforeClearDocumentCache;
                DomainService.Deleted += DomainService_Deleted;
                DomainService.Saved += DomainService_Saved;
            }
        }

        private void DomainService_Saved(IDomainService sender, SaveEventArgs<IDomain> e)
        {
            _umbracoHelper.ClearDomains();
        }

        private void DomainService_Deleted(IDomainService sender, DeleteEventArgs<IDomain> e)
        {
            _umbracoHelper.ClearDomains();
        }

        public void Terminate()
        {
            //maybe some clean up needed?
        }

        void ContentService_Deleting(IContentService sender, DeleteEventArgs<IContent> e)
        {
            foreach (IContent content in e.DeletedEntities)
            {
#if !DEBUG
                try
#endif
                {
                    UrlTrackerRepository.DeleteUrlTrackerEntriesByNodeId(content.Id);
                }
#if !DEBUG
                catch (Exception ex)
                {
                    ex.LogException();
                }
#endif
            }
        }

        void ContentService_Publishing(IContentService sender, ContentPublishingEventArgs e)
        {
            // When content is renamed or 'umbracoUrlName' property value is added/updated
            foreach (IContent content in e.PublishedEntities)
            {
#if !DEBUG
                try
#endif
                {
                    IPublishedContent node = _umbracoHelper.Content(content.Id);
                    if (node.Name != content.Name && !string.IsNullOrEmpty(node.Name)) // If name is null, it's a new document
                    {
                        // Rename occurred
                        UrlTrackerRepository.AddUrlMapping(content, node.Root().Id, node.Url, AutoTrackingTypes.Renamed);

                        if (ClientTools != null)
                            ClientTools.ChangeContentFrameUrl(string.Concat("/umbraco/editContent.aspx?id=", content.Id));
                    }
                    if (content.HasProperty("umbracoUrlName"))
                    {
                        string contentUmbracoUrlNameValue = content.GetValue("umbracoUrlName") != null ? content.GetValue("umbracoUrlName").ToString() : string.Empty;
                        string nodeUmbracoUrlNameValue = node.GetProperty("umbracoUrlName") != null ? node.GetProperty("umbracoUrlName").GetValue().ToString() : string.Empty;
                        if (contentUmbracoUrlNameValue != nodeUmbracoUrlNameValue)
                        {
                            // 'umbracoUrlName' property value added/changed
                            UrlTrackerRepository.AddUrlMapping(content, node.Root().Id, node.Url, AutoTrackingTypes.UrlOverwritten);

                            if (ClientTools != null)
                                ClientTools.ChangeContentFrameUrl(string.Concat("/umbraco/editContent.aspx?id=", content.Id));
                        }
                    }
                    if (UrlTrackerSettings.SEOMetadataInstalled && content.HasProperty(UrlTrackerSettings.SEOMetadataPropertyName))
                    {
                        string contentSEOMetadataValue = content.GetValue(UrlTrackerSettings.SEOMetadataPropertyName) != null ? content.GetValue(UrlTrackerSettings.SEOMetadataPropertyName).ToString() : string.Empty;
                        string nodeSEOMetadataValue = node.GetProperty("umbracoUrlName") != null ? node.GetProperty("umbracoUrlName").GetValue().ToString() : string.Empty;
                        if (contentSEOMetadataValue != nodeSEOMetadataValue)
                        {
                            dynamic contentJson = JObject.Parse(contentSEOMetadataValue);
                            string contentUrlName = contentJson.urlName;

                            dynamic nodeJson = JObject.Parse(nodeSEOMetadataValue);
                            string nodeUrlName = nodeJson.urlName;

                            if (contentUrlName != nodeUrlName)
                            {
                                // SEOMetadata UrlName property value added/changed
                                UrlTrackerRepository.AddUrlMapping(content, node.Root().Id, node.Url, AutoTrackingTypes.UrlOverwrittenSEOMetadata);
                                UrlTrackerRepository.AddUrlMapping(content, node.Root().Id, node.Url, AutoTrackingTypes.UrlOverwrittenSEOMetadata);

                                if (ClientTools != null)
                                    ClientTools.ChangeContentFrameUrl(string.Concat("/umbraco/editContent.aspx?id=", content.Id));
                            }
                        }
                    }
                }
#if !DEBUG
                catch (Exception ex)
                {
                    ex.LogException();
                }
#endif
            }
        }

        void ContentService_Published(IContentService sender, ContentPublishedEventArgs e)
        {
            foreach (IContent content in e.PublishedEntities)
            {
                UrlTrackerRepository.Convert410To301(content.Id);
            }
        }

        void ContentService_Moving(IContentService sender, MoveEventArgs<IContent> e)
        {
            IContent content = e.MoveInfoCollection.First().Entity; // i guess? 
#if !DEBUG
            try
#endif
            {
                if (content != null)
                {
                    IPublishedContent node = _umbracoHelper.Content(content.Id);
                    if (node != null && !string.IsNullOrEmpty(node.Url) && !content.Path.StartsWith("-1,-20")) // -1,-20 == Recycle bin | Not moved to recycle bin
                        UrlTrackerRepository.AddUrlMapping(content, node.Root().Id, node.Url, AutoTrackingTypes.Moved);
                }
            }
#if !DEBUG
            catch (Exception ex)
            {
                ex.LogException();
            }
#endif
        }

#pragma warning disable 0618
//        void content_BeforeClearDocumentCache(Document doc, DocumentCacheEventArgs e)
//#pragma warning restore
//        {
//#if !DEBUG
//            try
//#endif
//            {
//                UrlTrackerRepository.AddGoneEntryByNodeId(doc.Id);
//            }
//#if !DEBUG
//            catch (Exception ex)
//            {
//                ex.LogException();
//            }
//#endif
//        }

        //void Domain_New(Domain sender, NewEventArgs e)
        //{
        //    UmbracoHelper.ClearDomains();
        //}

        //void Domain_AfterSave(Domain sender, SaveEventArgs e)
        //{
        //    UmbracoHelper.ClearDomains();
        //}

        //void Domain_AfterDelete(Domain sender, umbraco.cms.businesslogic.DeleteEventArgs e)
        //{
        //    UmbracoHelper.ClearDomains();

        //}

    }
}