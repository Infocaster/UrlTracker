using InfoCaster.Umbraco.UrlTracker.Helpers;
using InfoCaster.Umbraco.UrlTracker.Models;
using InfoCaster.Umbraco.UrlTracker.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using InfoCaster.Umbraco.UrlTracker.Repositories;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;
using Umbraco.Web;
using System.Web;
using System.IO;
using System.Text;

namespace InfoCaster.Umbraco.UrlTracker.Services
{
	public class UrlTrackerService : IUrlTrackerService
	{
		private readonly IUmbracoContextFactory _umbracoContextFactory;
		private readonly IUrlTrackerCacheService _urlTrackerCacheService;
		private readonly IUrlTrackerSettings _urlTrackerSettings;
		private readonly IUrlTrackerHelper _urlTrackerHelper;
		private readonly IUrlTrackerRepository _urlTrackerRepository;
		private readonly IUrlTrackerLoggingHelper _urlTrackerLoggingHelper;
		private readonly IContentService _contentService;
		private readonly IDomainService _domainService;
		private readonly ILocalizationService _localizationService;

		private readonly string _forcedRedirectsCacheKey = "UrlTrackerForcedRedirects";
		private readonly string _urlTrackerDomainsCacheKey = "UrlTrackerDomains";

		public UrlTrackerService(
			IUmbracoContextFactory umbracoContextFactory,
			IUrlTrackerCacheService urlTrackerCacheService,
			IUrlTrackerSettings urlTrackerSettings,
			IDomainService domainService,
			IUrlTrackerHelper urlTrackerHelper,
			IUrlTrackerRepository urlTrackerRepository,
			IUrlTrackerLoggingHelper urlTrackerLoggingHelper,
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
				_urlTrackerRepository.DeleteNotFounds(entry.Culture, entry.OldUrl, entry.RedirectRootNodeId);

			entry.OldUrl = !string.IsNullOrEmpty(entry.OldUrl) ? _urlTrackerHelper.ResolveShortestUrl(entry.OldUrl) : null;
			entry.RedirectUrl = !string.IsNullOrEmpty(entry.RedirectUrl) ? _urlTrackerHelper.ResolveShortestUrl(entry.RedirectUrl) : null;
			entry.OldRegex = !string.IsNullOrEmpty(entry.OldRegex) ? entry.OldRegex : null;

			if (entry.ForceRedirect)
				ClearForcedRedirectsCache();

			return _urlTrackerRepository.AddEntry(entry);
		}

		public bool AddRedirect(IContent newContent, IPublishedContent oldContent, UrlTrackerHttpCode redirectType, UrlTrackerReason reason, string culture = null, bool isChild = false)
		{
			var oldUrl = string.IsNullOrEmpty(culture) ? oldContent.Url : oldContent.Url(culture);

			if (oldUrl == "#" || newContent.TemplateId <= 0)
				return false;

			var urlWithoutDomain = "";
			var domain = GetUmbracoDomainFromUrl(oldUrl, ref urlWithoutDomain);
			var rootNodeId = oldContent.Root().Id;
			var shortestOldUrl = _urlTrackerHelper.ResolveShortestUrl(urlWithoutDomain);

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
				Culture = !string.IsNullOrEmpty(culture) ? culture : null,
				RedirectHttpCode = (int)redirectType,
				RedirectRootNodeId = rootNodeId,
				RedirectNodeId = newContent.Id,
				OldUrl = shortestOldUrl,
				Notes = notes
			};

			_urlTrackerRepository.AddEntry(entry);

			if (entry.ForceRedirect)
				ClearForcedRedirectsCache();

			//Werkt dit met verschillende cultures?
			foreach (var child in oldContent.Children)
				AddRedirect(_contentService.GetById(child.Id), child, redirectType, reason, culture, true);

			return true;
		}

		public bool AddNotFound(string url, int rootNodeId, string referrer, string culture = null)
		{
			return _urlTrackerRepository.AddEntry(
				new UrlTrackerModel
				{
					Culture = culture,
					RedirectRootNodeId = rootNodeId,
					OldUrl = url,
					Referrer = referrer,
					Is404 = true
				}
			);
		}

		public UrlTrackerDomain GetUmbracoDomainFromUrl(string url, ref string urlWithoutDomain)
		{
			var domains = GetDomains();

			if (domains.Any())
			{
				var urlWithoutQuery = (url.Contains("?") ? url.Substring(0, url.IndexOf('?')) : url);
				urlWithoutQuery += !urlWithoutQuery.EndsWith("/") ? "/" : "";

				while (!string.IsNullOrEmpty(urlWithoutQuery))
				{
					if (urlWithoutQuery.EndsWith("/"))
					{
						var domain = domains.FirstOrDefault(x =>
							x.UrlWithDomain == urlWithoutQuery ||
							x.UrlWithDomain == urlWithoutQuery.TrimEnd('/') ||
							x.UrlWithDomain == urlWithoutQuery.Replace("http", "https") ||
							x.UrlWithDomain == urlWithoutQuery.Replace("https", "http")
						);

						if (domain != null)
						{
							urlWithoutDomain = url.Replace(domain.UrlWithDomain.Replace("http://", "https://"), "").Replace(domain.UrlWithDomain.Replace("https://", "http://"), "");
							return domain;
						}
					}

					urlWithoutQuery = urlWithoutQuery.Substring(0, urlWithoutQuery.Length - 1);
				}
			}

			urlWithoutDomain = url;
			return null;
		}

		public bool AddIgnore404(int id)
		{
			var entry = _urlTrackerRepository.GetEntryById(id);

			if (entry == null || !entry.Is404)
				return false;

			var result = _urlTrackerRepository.AddIgnore(new UrlTrackerIgnore404Schema
			{
				RootNodeId = entry.RedirectRootNodeId,
				Culture = entry.Culture,
				Url = entry.OldUrl
			});

			if (result)
				_urlTrackerRepository.DeleteNotFounds(entry.Culture, entry.OldUrl, entry.RedirectRootNodeId);

			return result;
		}

		public int ImportRedirects(HttpPostedFile file)
		{
			if(!file.FileName.EndsWith(".csv"))
				throw new Exception($"Is not a .csv file");

			var csvReader = new StreamReader(file.InputStream);
			var redirects = new List<UrlTrackerModel>();

			var columns = "RootNodeId;Culture;Old URL;Regex;Redirect URL;Redirect node ID;Redirect HTTP Code;Forward query;Force redirect;Notes";
			var lineNumber = 1;

			while (!csvReader.EndOfStream)
			{
				var line = csvReader.ReadLine();
				var values = line.Split(';');

				if (lineNumber == 1)
				{
					if (line == columns || line == columns.ToLower())
					{
						lineNumber++;
						continue;
					}
					else
					{
						throw new Exception($"Columns are incorrect");
					}
				}

				if (values.Count() != 10)
					throw new Exception($"Values on line: {lineNumber} are incomplete");

				int rootNodeId;
				int redirectNodeId = 0;
				int redirectHttpCode;
				bool forwardQuery = false;
				bool forceRedirect = false;

				string culture = values[1];
				string oldUrl = values[2];
				string regex = values[3];
				string redirectUrl = values[4];
				string notes = values[9];

				if (!Int32.TryParse(values[0], out rootNodeId))
					throw new Exception($"'RootNodeId' on line: {lineNumber} is not an integer");
				if (!string.IsNullOrWhiteSpace(values[5]) && !Int32.TryParse(values[5], out redirectNodeId))
					throw new Exception($"'Redirect node ID' on line: {lineNumber} is not a integer");
				if (!Int32.TryParse(values[6], out redirectHttpCode) || (redirectHttpCode != 301 && redirectHttpCode != 302 && redirectHttpCode != 410))
					throw new Exception($"'Redirect HTTP Code' on line: {lineNumber} is invalid");
				if (!string.IsNullOrWhiteSpace(values[7]) && !Boolean.TryParse(values[7], out forwardQuery))
					throw new Exception($"'Forward query' on line: {lineNumber} is invalid");
				if (!string.IsNullOrWhiteSpace(values[8]) && !Boolean.TryParse(values[8], out forceRedirect))
					throw new Exception($"'Force redirect' on line: {lineNumber} is invalid");

				var redirect = new UrlTrackerModel
				{
					RedirectRootNodeId = rootNodeId,
					Culture = culture,
					OldUrl = oldUrl,
					OldRegex = regex,
					RedirectUrl = redirectUrl,
					RedirectNodeId = redirectNodeId == 0 ? (int?)null : redirectNodeId,
					RedirectHttpCode = redirectHttpCode,
					RedirectPassThroughQueryString = forwardQuery,
					ForceRedirect = forceRedirect,
					Notes = notes
				};

				if (!ValidateRedirect(redirect))
					throw new Exception($"Missing required values on line: {lineNumber}");

				redirects.Add(redirect);

				lineNumber++;
			}

			foreach (var redirect in redirects)
				AddRedirect(redirect);

			return redirects.Count;
		}

		#endregion

		#region Get

		public UrlTrackerModel GetEntryById(int id)
		{
			return _urlTrackerRepository.GetEntryById(id);
		}

		public UrlTrackerGetResult GetRedirects(int skip, int amount, UrlTrackerSortType sortType = UrlTrackerSortType.CreatedDesc, string searchQuery = "")
		{
			return _urlTrackerRepository.GetRedirects(skip, amount, sortType, searchQuery);
		}

		public UrlTrackerGetResult GetNotFounds(int skip, int amount, UrlTrackerSortType sortType = UrlTrackerSortType.LastOccurredDesc, string searchQuery = "")
		{
			return _urlTrackerRepository.GetNotFounds(skip, amount, sortType, searchQuery);
		}

		public List<UrlTrackerModel> GetForcedRedirects()
		{
			var cachedForcedRedirects = _urlTrackerCacheService.Get<List<UrlTrackerModel>>(_forcedRedirectsCacheKey);

			if (cachedForcedRedirects == null)
			{
				var forcedRedirects = _urlTrackerRepository.GetRedirects(null, null, onlyForcedRedirects: true).Records;

				_urlTrackerCacheService.Set(
					_forcedRedirectsCacheKey,
					forcedRedirects,
					_urlTrackerSettings.IsForcedRedirectCacheTimeoutEnabled() ? _urlTrackerSettings.GetForcedRedirectCacheTimeoutSeconds() : (TimeSpan?)null
				);

				return forcedRedirects;
			}

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

		public IEnumerable<UrlTrackerLanguage> GetLanguagesOutNodeDomains(int nodeId)
		{
			var availableLanguages = new List<UrlTrackerLanguage>();
			var languages = _localizationService.GetAllLanguages();
			var domains = _domainService.GetAssignedDomains(nodeId, _urlTrackerSettings.HasDomainOnChildNode());

			foreach (var domain in domains)
			{
				if (!availableLanguages.Any(x => x.IsoCode == domain.LanguageIsoCode.ToLower()))
				{
					availableLanguages.Add(languages.Where(x => x.IsoCode == domain.LanguageIsoCode)
						.Select(x => new UrlTrackerLanguage
						{
							Id = x.Id,
							IsoCode = x.IsoCode.ToLower(),
							CultureName = x.CultureName
						}).First());
				}
			}

			return availableLanguages;
		}

		public int CountNotFoundsThisWeek()
		{
			var startDate = DateTime.Now.Date.AddDays(-((int)DateTime.Now.DayOfWeek - 1));
			var endDate = DateTime.Now;

			return _urlTrackerRepository.CountNotFoundsBetweenDates(startDate, endDate);
		}

		public bool IgnoreExist(string url, int RootNodeId, string culture)
		{
			return _urlTrackerRepository.IgnoreExist(url, RootNodeId, culture);
		}

		public string GetRedirectsCsv()
		{
			var redirects = _urlTrackerRepository.GetRedirects(null, null);
			var csv = new StringBuilder();

			csv.AppendLine("RootNodeId;Culture;Old URL;Regex;Redirect URL;Redirect node ID;Redirect HTTP Code;Forward query;Force redirect;Notes");

			foreach (var redirect in redirects.Records)
				csv.AppendLine($"{redirect.RedirectRootNodeId};{redirect.Culture};{redirect.OldUrl};{redirect.OldRegex};{redirect.RedirectUrl};{redirect.RedirectNodeId};{redirect.RedirectHttpCode};{redirect.RedirectPassThroughQueryString};{redirect.ForceRedirect};{redirect.Notes}");

			return csv.ToString();
		}

		#endregion

		#region Update

		public void UpdateEntry(UrlTrackerModel entry)
		{
			entry.OldUrl = !string.IsNullOrEmpty(entry.OldUrl) ? _urlTrackerHelper.ResolveShortestUrl(entry.OldUrl) : null;
			entry.RedirectUrl = !string.IsNullOrEmpty(entry.RedirectUrl) ? _urlTrackerHelper.ResolveShortestUrl(entry.RedirectUrl) : null;
			entry.OldRegex = !string.IsNullOrEmpty(entry.OldRegex) ? entry.OldRegex : null;

			_urlTrackerRepository.UpdateEntry(entry);

			if (entry.ForceRedirect)
				ClearForcedRedirectsCache();
		}

		public void Convert410To301ByNodeId(int nodeId)
		{
			_urlTrackerRepository.Convert410To301ByNodeId(nodeId);
		}

		public void ConvertRedirectTo410ByNodeId(int nodeId)
		{
			_urlTrackerRepository.ConvertRedirectTo410ByNodeId(nodeId);
		}

		public void ClearDomains()
		{
			_urlTrackerCacheService.Clear(_urlTrackerDomainsCacheKey);
		}

		public void ClearForcedRedirectsCache()
		{
			_urlTrackerCacheService.Clear(_forcedRedirectsCacheKey);
		}

		#endregion

		#region Delete

		public bool DeleteEntryById(int id, bool is404)
		{
			var entry = _urlTrackerRepository.GetEntryById(id);

			if(entry != null)
			{
				if (is404)
				{
					_urlTrackerRepository.DeleteNotFounds(entry.Culture, entry.OldUrl, entry.RedirectRootNodeId);
				}

				_urlTrackerRepository.DeleteEntryById(id);

				if(entry.ForceRedirect)
				{
					ClearForcedRedirectsCache();
				}
			}

			return true;
		}

		public void DeleteEntryByRedirectNodeId(int nodeId)
		{
			if (_urlTrackerRepository.DeleteEntryByRedirectNodeId(nodeId))
				ClearForcedRedirectsCache();
		}

		#endregion

		public bool ValidateRedirect(UrlTrackerModel redirect)
		{
			if ((string.IsNullOrEmpty(redirect.OldUrl) && string.IsNullOrEmpty(redirect.OldRegex)) ||
				(redirect.RedirectRootNodeId == 0 || redirect.RedirectRootNodeId == null) ||
				((redirect.RedirectNodeId == null || redirect.RedirectNodeId == 0) && string.IsNullOrEmpty(redirect.RedirectUrl)) ||
				(redirect.RedirectHttpCode != 301 && redirect.RedirectHttpCode != 302 && redirect.RedirectHttpCode != 410))
				return false;

			return true;
		}
	}
}