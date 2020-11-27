using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InfoCaster.Umbraco.UrlTracker.Helpers;
using InfoCaster.Umbraco.UrlTracker.Models;
using InfoCaster.Umbraco.UrlTracker.NewRepositories;
using InfoCaster.Umbraco.UrlTracker.Settings;
using Lucene.Net.Util;
using Umbraco.Core.Configuration;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Security;

namespace InfoCaster.Umbraco.UrlTracker.Services
{
	public class UrlTrackerService : IUrlTrackerService
	{
		private readonly IUmbracoContextFactory _umbracoContextFactory;
		private readonly IUrlTrackerCacheService _urlTrackerCacheService;
		private readonly IUrlTrackerNewSettings _urlTrackerSettings;
		private readonly IUrlTrackerNewHelper _urlTrackerHelper;
		private readonly IUrlTrackerNewRepository _urlTrackerRepository;
		private readonly IUrlTrackerNewLoggingHelper _urlTrackerLoggingHelper;
		private readonly IContentService _contentService;
		private readonly IDomainService _domainService;
		private readonly ILocalizationService _localizationService;

		private readonly string _forcedRedirectsCacheKey = "UrlTrackerForcedRedirects";
		private readonly string _urlTrackerDomainsCacheKey = "UrlTrackerDomains";

		public UrlTrackerService(
			IUmbracoContextFactory umbracoContextFactory,
			IUrlTrackerCacheService urlTrackerCacheService,
			IUrlTrackerNewSettings urlTrackerSettings,
			IDomainService domainService,
			IUrlTrackerNewHelper urlTrackerHelper,
			IUrlTrackerNewRepository urlTrackerRepository,
			IUrlTrackerNewLoggingHelper urlTrackerLoggingHelper,
			IContentService contentService,
			ILocalizationService localizationService)
		{
			_umbracoContextFactory = umbracoContextFactory;
			_urlTrackerCacheService = urlTrackerCacheService;
			_urlTrackerSettings = urlTrackerSettings;
			_domainService = domainService;
			_urlTrackerHelper = urlTrackerHelper;
			_urlTrackerRepository = urlTrackerRepository;
			_urlTrackerLoggingHelper = urlTrackerLoggingHelper;
			_contentService = contentService;
			_localizationService = localizationService;
		}

		#region Add

		public bool AddRedirect(UrlTrackerModel entry)
		{
			if (entry.Remove404)
			{
				_urlTrackerRepository.DeleteNotFounds(entry.OldUrl, entry.RedirectRootNodeId);
				entry.Is404 = false;
			}

			return _urlTrackerRepository.AddEntry(entry);
		}

		public bool AddRedirect(IContent newContent, IPublishedContent oldContent, UrlTrackerRedirectType redirectType, UrlTrackerReason reason, string culture = "", bool isChild = false)
		{
			var oldUrl = string.IsNullOrEmpty(culture) ? oldContent.Url : oldContent.Url(culture);

			if (oldUrl == "#" || newContent.TemplateId <= 0)
				return false;

			var rootNodeId = oldContent.Root().Id;
			var shortestOldUrl = _urlTrackerHelper.ResolveShortestUrl(oldUrl);

			if (shortestOldUrl == "/" || string.IsNullOrEmpty(shortestOldUrl) || _urlTrackerRepository.RedirectExist(newContent.Id, shortestOldUrl, culture))
				return false;

			string notes = isChild ? "An ancestor" : "This page";

			if (reason == UrlTrackerReason.Moved)
				notes += " was moved";
			else if (reason == UrlTrackerReason.Renamed)
				notes += " was renamed";
			else if (reason == UrlTrackerReason.UrlOverwritten)
				notes += "'s property 'umbracoUrlName' changed";
			else if (reason == UrlTrackerReason.UrlOverwrittenSEOMetadata)
				notes += $"'s property '{_urlTrackerSettings.GetSEOMetadataPropertyName()}' changed";

			if (_urlTrackerSettings.HasDomainOnChildNode())
			{
				var rootUri = new Uri(GetUrlByNodeId(rootNodeId));
				var shortRootUrl = _urlTrackerHelper.ResolveShortestUrl(rootUri.AbsolutePath);

				if (shortestOldUrl.StartsWith(shortRootUrl, StringComparison.OrdinalIgnoreCase))
					shortestOldUrl = _urlTrackerHelper.ResolveShortestUrl(shortestOldUrl.Substring(shortRootUrl.Length));
			}

			_urlTrackerLoggingHelper.LogInformation("UrlTracker Repository | Adding mapping for node id: {0} and url: {1}", oldContent.Id.ToString(), shortestOldUrl);

			var entry = new UrlTrackerModel
			{
				Culture = !string.IsNullOrEmpty(culture) ? culture.ToLower() : null,
				RedirectHttpCode = (int)redirectType,
				RedirectRootNodeId = rootNodeId,
				RedirectNodeId = newContent.Id,
				OldUrl = shortestOldUrl,
				Notes = notes
			};

			_urlTrackerRepository.AddEntry(entry);

			//Werkt dit met verschillende cultures?
			foreach (var child in oldContent.Children)
				AddRedirect(_contentService.GetById(child.Id), child, redirectType, reason, culture, true);

			return true;
		}

		public bool AddNotFound(string url, int rootNodeId, string referrer)
		{
			return _urlTrackerRepository.AddEntry(
				new UrlTrackerModel
				{
					RedirectRootNodeId = rootNodeId,
					OldUrl = url,
					Referrer = referrer,
					Is404 = true
				}
			);
		}

		#endregion

		#region Get

		public UrlTrackerModel GetEntryById(int id)
		{
			return _urlTrackerRepository.GetEntryById(id);
		}

		public UrlTrackerGetResult GetRedirects(int skip, int amount)
		{
			return _urlTrackerRepository.GetRedirects(skip, amount);
		}

		public UrlTrackerGetResult GetRedirectsByFilter(int skip, int amount, UrlTrackerSortType sortType = UrlTrackerSortType.CreatedDesc, string searchQuery = "")
		{
			return _urlTrackerRepository.GetRedirects(skip, amount, sortType, searchQuery);
		}

		public UrlTrackerGetResult GetNotFounds(int skip, int amount)
		{
			return _urlTrackerRepository.GetNotFounds(skip, amount);
		}

		public UrlTrackerGetResult GetNotFoundsByFilter(int skip, int amount, UrlTrackerSortType sortType = UrlTrackerSortType.LastOccurrenceDesc, string searchQuery = "")
		{
			return _urlTrackerRepository.GetNotFounds(skip, amount, sortType, searchQuery);
		}

		public List<UrlTrackerModel> GetForcedRedirects()
		{
			var cachedForcedRedirects = _urlTrackerCacheService.Get<List<UrlTrackerModel>>(_forcedRedirectsCacheKey);

			if (cachedForcedRedirects == null)
				return ReloadForcedRedirectsCache();

			return cachedForcedRedirects;
		}

		public List<UrlTrackerDomain> GetDomains()
		{
			var urlTrackerDomains = _urlTrackerCacheService.Get<List<UrlTrackerDomain>>(_urlTrackerDomainsCacheKey);

			if (urlTrackerDomains == null)
			{
				var umbDomains = _domainService.GetAll(_urlTrackerSettings.HasDomainOnChildNode());
				urlTrackerDomains = new List<UrlTrackerDomain>();

				foreach (var umbDomain in umbDomains)
				{
					urlTrackerDomains.Add(new UrlTrackerDomain
					{
						Id = umbDomain.Id,
						NodeId = umbDomain.RootContentId.Value,
						Name = umbDomain.DomainName,
						LanguageIsoCode = umbDomain.LanguageIsoCode
					});
				}

				urlTrackerDomains = urlTrackerDomains.OrderBy(x => x.Name).ToList();
				_urlTrackerCacheService.Set(_urlTrackerDomainsCacheKey, urlTrackerDomains);
			}

			return urlTrackerDomains;
		}

		public string GetUrlByNodeId(int nodeId, string culture = "")
		{
			using (var ctx = _umbracoContextFactory.EnsureUmbracoContext())
			{
				return ctx.UmbracoContext.Content.GetById(nodeId)?.Url(!string.IsNullOrEmpty(culture) ? culture : null);
			}
		}

		public IPublishedContent GetNodeById(int nodeId)
		{
			using (var ctx = _umbracoContextFactory.EnsureUmbracoContext())
			{
				return ctx.UmbracoContext.Content.GetById(nodeId);
			}
		}

		public bool RedirectExist(int redirectNodeId, string oldUrl, string culture = "")
		{
			return _urlTrackerRepository.RedirectExist(redirectNodeId, oldUrl, culture);
		}

		public IEnumerable<UrlTrackerLanguage> GetLanguages()
		{
			return _localizationService.GetAllLanguages().Select(x => new UrlTrackerLanguage
			{
				Id = x.Id,
				IsoCode = x.IsoCode.ToLower(),
				CultureName = x.CultureName
			});
		}

		#endregion

		#region Update

		public void UpdateEntry(UrlTrackerModel entry)
		{
			_urlTrackerRepository.UpdateEntry(entry);

			if (entry.ForceRedirect)
				ReloadForcedRedirectsCache();
		}

		public void Convert410To301ByNodeId(int nodeId)
		{
			_urlTrackerRepository.Convert410To301ByNodeId(nodeId);
		}

		public void ClearDomains()
		{
			_urlTrackerCacheService.Clear(_urlTrackerDomainsCacheKey);
		}

		public List<UrlTrackerModel> ReloadForcedRedirectsCache()
		{
			var forcedRedirects = _urlTrackerRepository.GetRedirects(null, null, onlyForcedRedirects: true).Records;

			_urlTrackerCacheService.Set(
				_forcedRedirectsCacheKey,
				forcedRedirects,
				_urlTrackerSettings.IsForcedRedirectCacheTimeoutEnabled() ? _urlTrackerSettings.GetForcedRedirectCacheTimeoutSeconds() : (TimeSpan?)null
			);

			return forcedRedirects;
		}

		#endregion

		#region Delete

		public void DeleteEntryById(int id, bool is404)
		{
			if (is404)
			{
				var entry = _urlTrackerRepository.GetEntryById(id);
				_urlTrackerRepository.DeleteNotFounds(entry.OldUrl, entry.RedirectRootNodeId);
			}
			else
			{
				_urlTrackerRepository.DeleteEntryById(id);
			}
		}

		public void DeleteEntryByRedirectNodeId(int nodeId)
		{
			if (_urlTrackerRepository.DeleteEntryByRedirectNodeId(nodeId))
				ReloadForcedRedirectsCache();
		}

		#endregion
	}
}