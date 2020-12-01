using InfoCaster.Umbraco.UrlTracker.Extensions;
using InfoCaster.Umbraco.UrlTracker.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using InfoCaster.Umbraco.UrlTracker.Helpers;
using InfoCaster.Umbraco.UrlTracker.Repositories;
using InfoCaster.Umbraco.UrlTracker.Services;
using InfoCaster.Umbraco.UrlTracker.Settings;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Events;
using Umbraco.Core.Logging;
using Umbraco.Core.Migrations;
using Umbraco.Core.Migrations.Upgrade;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Implement;
using Umbraco.Web;

namespace InfoCaster.Umbraco.UrlTracker
{

	[RuntimeLevel(MinLevel = RuntimeLevel.Run)]
	public class UrlTrackerComposer : IUserComposer
	{
		public void Compose(Composition composition)
		{
			composition.Components().Append<UrlTrackerComponent>();

			composition.Register<IUrlTrackerHelper, UrlTrackerHelper>();
			composition.Register<IUrlTrackerLoggingHelper, UrlTrackerLoggingHelper>();
			composition.Register<IUrlTrackerRepository, UrlTrackerRepository>();
			composition.Register<IUrlTrackerCacheService, UrlTrackerCacheService>();
			composition.Register<IUrlTrackerService, UrlTrackerService>();

			composition.Register<IUrlTrackerSettings, UrlTrackerSettings>(Lifetime.Singleton);
		}
	}

	public class UrlTrackerComponent : IComponent
	{
		private readonly IScopeProvider _scopeProvider;
		private readonly IMigrationBuilder _migrationBuilder;
		private readonly IKeyValueService _keyValueService;
		private readonly ILogger _logger;
		private readonly IUrlTrackerService _urlTrackerService;
		private readonly IUrlTrackerSettings _urlTrackerSettings;

		public UrlTrackerComponent(
			IUrlTrackerSettings urlTrackerSettings,
			IUrlTrackerService urlTrackerService,
			IScopeProvider scopeProvider,
			IMigrationBuilder migrationBuilder,
			IKeyValueService keyValueService,
			ILogger logger)
		{
			_urlTrackerSettings = urlTrackerSettings;
			_scopeProvider = scopeProvider;
			_migrationBuilder = migrationBuilder;
			_keyValueService = keyValueService;
			_logger = logger;
			_urlTrackerService = urlTrackerService;
		}

		public void Initialize()
		{
			var migrationPlan = new MigrationPlan("UrlTracker");
			migrationPlan.From(string.Empty).To<AddUrlTrackerTable>("urlTracker");

			var upgrader = new Upgrader(migrationPlan);
			upgrader.Execute(_scopeProvider, _migrationBuilder, _keyValueService, _logger);


			if (!_urlTrackerSettings.IsDisabled() && !_urlTrackerSettings.IsTrackingDisabled())
			{
				//todo: resolve check from migration and execute this

				ContentService.Moving += ContentService_Moving;
				ContentService.Publishing += ContentService_Publishing;
				ContentService.Published += ContentService_Published;

				ContentService.Deleting += ContentService_Deleting;
				DomainService.Deleted += DomainService_Deleted;
				DomainService.Saved += DomainService_Saved;
			}
		}

		public void Terminate() { }

		private void DomainService_Saved(IDomainService sender, SaveEventArgs<IDomain> e)
		{
			_urlTrackerService.ClearDomains();
		}

		private void DomainService_Deleted(IDomainService sender, DeleteEventArgs<IDomain> e)
		{
			_urlTrackerService.ClearDomains();
		}

		void ContentService_Deleting(IContentService sender, DeleteEventArgs<IContent> e)
		{
			foreach (IContent content in e.DeletedEntities)
			{
#if !DEBUG
                try
#endif
				{
					_urlTrackerService.DeleteEntryByRedirectNodeId(content.Id);
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
			foreach (IContent newContent in e.PublishedEntities)
			{
#if !DEBUG
                try
#endif
				{
					var oldContent = _urlTrackerService.GetNodeById(newContent.Id);

					if (oldContent != null) // If old content is null, it's a new document
					{
						if (newContent.AvailableCultures.Any())
						{
							foreach (var culture in newContent.PublishedCultures)
								MatchNodes(newContent, oldContent, culture);
						}
						else
						{
							MatchNodes(newContent, oldContent);
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
				_urlTrackerService.Convert410To301ByNodeId(content.Id);
			}
		}

		void ContentService_Moving(IContentService sender, MoveEventArgs<IContent> e)
		{
#if !DEBUG
            try
#endif
			foreach (var moved in e.MoveInfoCollection)
			{
				IContent newContent = moved.Entity;

				if (newContent == null)
					return;

				var oldContent = _urlTrackerService.GetNodeById(newContent.Id);

				if (oldContent != null && !string.IsNullOrEmpty(oldContent.Url) && oldContent.Parent.Id != moved.NewParentId)
				{
					if (newContent.AvailableCultures.Any())
					{
						foreach (var culture in newContent.AvailableCultures)
							_urlTrackerService.AddRedirect(newContent, oldContent, UrlTrackerRedirectType.MovedPermanently, UrlTrackerReason.Moved, culture);
					}
					else
					{
						_urlTrackerService.AddRedirect(newContent, oldContent, UrlTrackerRedirectType.MovedPermanently, UrlTrackerReason.Moved);
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

		private void MatchNodes(IContent newContent, IPublishedContent oldContent, string culture = "")
		{
			var newContentName = string.IsNullOrEmpty(culture) ? newContent.Name : newContent.CultureInfos[culture].Name;

			if (!string.IsNullOrEmpty(oldContent.Name(culture)) && newContentName != oldContent.Name(culture)) // 'Name' changed
				_urlTrackerService.AddRedirect(newContent, oldContent, UrlTrackerRedirectType.MovedPermanently, UrlTrackerReason.Renamed, culture);

			if (newContent.HasProperty("umbracoUrlName"))
			{
				var newContentUmbracoUrlName = newContent.GetValue("umbracoUrlName", culture)?.ToString() ?? "";
				var oldContentUmbracoUrlName = oldContent.Value("umbracoUrlName", culture)?.ToString() ?? "";

				if (!newContentUmbracoUrlName.Equals(oldContentUmbracoUrlName))  // 'umbracoUrlName' property value added/changed
					_urlTrackerService.AddRedirect(newContent, oldContent, UrlTrackerRedirectType.MovedPermanently, UrlTrackerReason.UrlOverwritten, culture);
			}

			if (_urlTrackerSettings.IsSEOMetadataInstalled() && newContent.HasProperty(_urlTrackerSettings.GetSEOMetadataPropertyName()))
			{
				var newContentSEOMetadata = newContent.GetValue(_urlTrackerSettings.GetSEOMetadataPropertyName(), culture)?.ToString() ?? "";
				var oldContentSEOMetadata = oldContent.Value(_urlTrackerSettings.GetSEOMetadataPropertyName(), culture)?.ToString() ?? "";

				if (!newContentSEOMetadata.Equals(oldContentSEOMetadata))
				{
					dynamic contentJson = JObject.Parse(newContentSEOMetadata);
					string newContentUrlName = contentJson.urlName;

					dynamic nodeJson = JObject.Parse(oldContentSEOMetadata);
					string oldContentUrlName = nodeJson.urlName;

					if (newContentUrlName != oldContentUrlName) // SEOMetadata UrlName property value added/changed
						_urlTrackerService.AddRedirect(newContent, oldContent, UrlTrackerRedirectType.MovedPermanently, UrlTrackerReason.UrlOverwrittenSEOMetadata, culture);
				}
			}
		}
	}

	public class AddUrlTrackerTable : MigrationBase
	{
		public AddUrlTrackerTable(IMigrationContext context) : base(context) { }

		public override void Migrate()
		{
			Logger.Debug<AddUrlTrackerTable>("Running migration {MigrationStep}", "AddUrlTrackerTable");

			if (!TableExists("icUrlTracker"))
				Create.Table<UrlTrackerSchema>().Do();
			else
				Logger.Debug<AddUrlTrackerTable>("The database table {DbTable} already exists, skipping", "icUrlTracker");
		}

	}
}