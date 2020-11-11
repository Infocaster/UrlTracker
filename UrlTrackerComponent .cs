using InfoCaster.Umbraco.UrlTracker.Extensions;
using InfoCaster.Umbraco.UrlTracker.Models;
using InfoCaster.Umbraco.UrlTracker.NewRepositories;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using InfoCaster.Umbraco.UrlTracker.Helpers;
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

			composition.Register<IUrlTrackerNewHelper, UrlTrackerNewHelper>();
			composition.Register<IUrlTrackerNewLoggingHelper, UrlTrackerNewLoggingHelper>();
			composition.Register<IUrlTrackerNewRepository, UrlTrackerNewRepository>();
			composition.Register<IUrlTrackerCacheService, UrlTrackerCacheService>();
			composition.Register<IUrlTrackerService, UrlTrackerService>();

			composition.Register<IUrlTrackerNewSettings, UrlTrackerNewSettings>(Lifetime.Singleton);
		}
	}

	public class UrlTrackerComponent : IComponent
	{
		private readonly IScopeProvider _scopeProvider;
		private readonly IMigrationBuilder _migrationBuilder;
		private readonly IKeyValueService _keyValueService;
		private readonly ILogger _logger;
		private readonly IUrlTrackerService _urlTrackerService;
		private readonly IUrlTrackerNewSettings _urlTrackerSettings;
		private readonly IUrlTrackerNewRepository _urlTrackerRepository;

		public UrlTrackerComponent(
			IUrlTrackerNewSettings urlTrackerSettings,
			IUrlTrackerNewRepository urlTrackerRepository,
			IUrlTrackerService urlTrackerService,
			IScopeProvider scopeProvider,
			IMigrationBuilder migrationBuilder,
			IKeyValueService keyValueService,
			ILogger logger)
		{
			_urlTrackerSettings = urlTrackerSettings;
			_urlTrackerRepository = urlTrackerRepository;
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
					_urlTrackerRepository.DeleteEntryByRedirectNodeId(content.Id);
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

					if (oldContent?.Name != newContent.Name && !string.IsNullOrEmpty(oldContent?.Name)) // If name is null, it's a new document
						_urlTrackerService.AddRedirect(newContent, oldContent, UrlTrackerRedirectType.MovedPermanently, UrlTrackerReason.Renamed);

					if (newContent.HasProperty("umbracoUrlName"))
					{
						var newContentUmbracoUrlName = newContent.GetValue("umbracoUrlName") != null ? newContent.GetValue("umbracoUrlName").ToString() : string.Empty;
						var oldContentUmbracoUrlName = oldContent.GetProperty("umbracoUrlName") != null ? oldContent.GetProperty("umbracoUrlName").GetValue().ToString() : string.Empty;

						if (newContentUmbracoUrlName != oldContentUmbracoUrlName)  // 'umbracoUrlName' property value added/changed
							_urlTrackerService.AddRedirect(newContent, oldContent, UrlTrackerRedirectType.MovedPermanently, UrlTrackerReason.UrlOverwritten);
					}

					if (_urlTrackerSettings.IsSEOMetadataInstalled() && newContent.HasProperty(_urlTrackerSettings.GetSEOMetadataPropertyName()))
					{
						var newContentSEOMetadata = newContent.GetValue(_urlTrackerSettings.GetSEOMetadataPropertyName()) != null ? newContent.GetValue(_urlTrackerSettings.GetSEOMetadataPropertyName()).ToString() : string.Empty;
						var oldContentSEOMetadata = oldContent.GetProperty("umbracoUrlName") != null ? oldContent.GetProperty("umbracoUrlName").GetValue().ToString() : string.Empty;

						if (newContentSEOMetadata != oldContentSEOMetadata)
						{
							dynamic contentJson = JObject.Parse(newContentSEOMetadata);
							string newContentUrlName = contentJson.urlName;

							dynamic nodeJson = JObject.Parse(oldContentSEOMetadata);
							string oldContentUrlName = nodeJson.urlName;

							if (newContentUrlName != oldContentUrlName) // SEOMetadata UrlName property value added/changed
								_urlTrackerService.AddRedirect(newContent, oldContent, UrlTrackerRedirectType.MovedPermanently, UrlTrackerReason.UrlOverwrittenSEOMetadata);
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
				_urlTrackerRepository.Convert410To301ByNodeId(content.Id);
			}
		}

		void ContentService_Moving(IContentService sender, MoveEventArgs<IContent> e)
		{
			IContent newContent = e.MoveInfoCollection.First().Entity;

			if (newContent == null)
				return;
#if !DEBUG
            try
#endif
			{
				var oldContent = _urlTrackerService.GetNodeById(newContent.Id);

				if (oldContent != null && !string.IsNullOrEmpty(oldContent.Url) && !newContent.Path.StartsWith("-1,-20")) // -1,-20 == Recycle bin | Not moved to recycle bin
					_urlTrackerService.AddRedirect(newContent, oldContent, UrlTrackerRedirectType.MovedPermanently, UrlTrackerReason.Moved);
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