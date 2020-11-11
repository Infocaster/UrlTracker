using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InfoCaster.Umbraco.UrlTracker.Helpers;
using InfoCaster.Umbraco.UrlTracker.Models;
using InfoCaster.Umbraco.UrlTracker.NewRepositories;
using InfoCaster.Umbraco.UrlTracker.Settings;
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

		private readonly string _urlTrackerDomainsCacheKey = "UrlTrackerDomains";

		public UrlTrackerService(
			IUmbracoContextFactory umbracoContextFactory,
			IUrlTrackerCacheService urlTrackerCacheService,
			IUrlTrackerNewSettings urlTrackerSettings,
			IDomainService domainService,
			IUrlTrackerNewHelper urlTrackerHelper,
			IUrlTrackerNewRepository urlTrackerRepository,
			IUrlTrackerNewLoggingHelper urlTrackerLoggingHelper,
			IContentService contentService)
		{
			_umbracoContextFactory = umbracoContextFactory;
			_urlTrackerCacheService = urlTrackerCacheService;
			_urlTrackerSettings = urlTrackerSettings;
			_domainService = domainService;
			_urlTrackerHelper = urlTrackerHelper;
			_urlTrackerRepository = urlTrackerRepository;
			_urlTrackerLoggingHelper = urlTrackerLoggingHelper;
			_contentService = contentService;
		}


		public bool AddRedirect(IContent newContent, IPublishedContent oldContent, UrlTrackerRedirectType redirectType, UrlTrackerReason reason, bool isChild = false)
		{
			if (oldContent.Url == "#" || newContent.TemplateId <= 0)
				return false;

			var rootNodeId = oldContent.Root().Id;
			var oldUrl = _urlTrackerHelper.ResolveShortestUrl(oldContent.Url);

			if (string.IsNullOrEmpty(oldUrl) || _urlTrackerRepository.RedirectExist(newContent.Id, oldUrl))
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

				if (oldUrl.StartsWith(shortRootUrl, StringComparison.OrdinalIgnoreCase))
					oldUrl = _urlTrackerHelper.ResolveShortestUrl(oldUrl.Substring(shortRootUrl.Length));
			}

			_urlTrackerLoggingHelper.LogInformation("UrlTracker Repository | Adding mapping for node id: {0} and url: {1}", oldContent.Id.ToString(), oldUrl);

			var entry = new UrlTrackerModel
			{
				RedirectHttpCode = (int)redirectType,
				RedirectRootNodeId = rootNodeId,
				RedirectNodeId = newContent.Id,
				OldUrl = oldUrl,
				Notes = notes
			};

			_urlTrackerRepository.AddEntry(entry);

			foreach (var child in oldContent.Children)
				AddRedirect(_contentService.GetById(child.Id), child, redirectType, reason, true);

			return true;
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
					urlTrackerDomains.Add(new UrlTrackerDomain(umbDomain.Id, umbDomain.RootContentId.Value, umbDomain.DomainName));
				}

				urlTrackerDomains = urlTrackerDomains.OrderBy(x => x.Name).ToList();
				_urlTrackerCacheService.Set(_urlTrackerDomainsCacheKey, urlTrackerDomains);
			}

			return urlTrackerDomains;
		}

		public void ClearDomains()
		{
			_urlTrackerCacheService.Clear(_urlTrackerDomainsCacheKey);
		}

		public string GetUrlByNodeId(int nodeId)
		{
			using (var ctx = _umbracoContextFactory.EnsureUmbracoContext())
			{
				return ctx.UmbracoContext.Content.GetById(nodeId)?.Url;
			}
		}

		public IPublishedContent GetNodeById(int nodeId)
		{
			using (var ctx = _umbracoContextFactory.EnsureUmbracoContext())
			{
				return ctx.UmbracoContext.Content.GetById(nodeId);
			}
		}
	}
}