using InfoCaster.Umbraco.UrlTracker.Extensions;
using InfoCaster.Umbraco.UrlTracker.Models;
using InfoCaster.Umbraco.UrlTracker.Repositories;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
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
using Umbraco.Core.Logging;
using Umbraco.Core.Migrations;
using Umbraco.Core.Migrations.Upgrade;
using Umbraco.Core.Scoping;
using NPoco;
using Umbraco.Core.Persistence.DatabaseAnnotations;
using Microsoft.Owin.Security.Provider;
using System.Web.UI.WebControls;
using InfoCaster.Umbraco.UrlTracker;
using InfoCaster.Umbraco.UrlTracker.Modules;
using InfoCaster.Umbraco.UrlTracker.Providers;
using System.Web.Hosting;

namespace InfoCaster.Umbraco.UrlTracker
{

    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class UrlTrackerComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Components().Append<UrlTrackerComponent>();
        }
    }

    public class UrlTrackerComponent : IComponent
    {
        private UmbracoHelper _umbracoHelper;

        private IScopeProvider _scopeProvider;
        private IMigrationBuilder _migrationBuilder;
        private IKeyValueService _keyValueService;
        private ILogger _logger;

        public UrlTrackerComponent(IScopeProvider scopeProvider, IMigrationBuilder migrationBuilder, IKeyValueService keyValueService, ILogger logger)
        {
            _scopeProvider = scopeProvider;
            _migrationBuilder = migrationBuilder;
            _keyValueService = keyValueService;
            _logger = logger;
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
            var migrationPlan = new MigrationPlan("UrlTracker");
            migrationPlan.From(string.Empty)
                .To<AddUrlTrackerTable>("urlTracker");

            var upgrader = new Upgrader(migrationPlan);
            upgrader.Execute(_scopeProvider, _migrationBuilder, _keyValueService, _logger);
            
            if (!UrlTrackerSettings.IsDisabled && !UrlTrackerSettings.IsTrackingDisabled)
            {
                //todo: resolve check from migration and execute this
                //UrlTrackerRepository.ReloadForcedRedirectsCache();
                
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
                    if (_umbracoHelper == null)
                    {
                        _umbracoHelper = Current.Factory.TryGetInstance<UmbracoHelper>();
                    }
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

    }

    public class AddUrlTrackerTable : MigrationBase
    {
        public AddUrlTrackerTable(IMigrationContext context) : base(context)
        {
        }

        public override void Migrate()
        {
            Logger.Debug<AddUrlTrackerTable>("Running migration {MigrationStep}", "AddUrlTrackerTable");
            if(!TableExists("icUrlTracker"))
            {
                Create.Table<UrlTrackerSchema>().Do();
            }
            else
            {
                Logger.Debug<AddUrlTrackerTable>("The database table {DbTable} already exists, skipping","icUrlTracker");
            }
        }

        [TableName("icUrlTracker")]
        [PrimaryKey("Id", AutoIncrement = true)]
        [ExplicitColumns]
        public class UrlTrackerSchema
        {
            [PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 1)]
            [Column("Id")]
            public int Id { get; set; }

            [Column("OldUrl")]
            public string OldUrl { get; set; }

            [Column("OldUrlQueryString")]
            public string OldUrlQueryString { get; set; }

            [Column("OldRegex")]
            public string OldRexEx { get; set; }

            [Column("RedirectRootNodeId")]
            public int RedirectRootNodeId { get; set; }

            [Column("RedirectNodeId")]
            public int RedirectNodeId { get; set; }
            
            [Column("RedirectUrl")]
            public string RedirectUrl { get; set; }

            [Column("RedirectHttpCode")]
            public int RedirectHttpCode { get; set; }
            
            [Column("RedirectPassThroughQueryString")]
            public bool RedirectPassThroughQueryString { get; set; }
            
            [Column("ForceRedirect")]
            public bool ForceRedirect { get; set; }

            [Column("Notes")]
            public string Notes { get; set; }

            [Column("Is404")]
            public bool Is404 { get; set; }

            [Column("Referrer")]
            public string Referred { get; set; }

            [Column("Inserted")]
            public DateTime Inserted { get; set; }
        }
    }
}