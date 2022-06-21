using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Scoping;
using UrlTracker.Core.Caching;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Core.Database.Models;

namespace UrlTracker.Resources.Website.Preset
{
    public interface IPresetService
    {
        void EnsureContent();
        void Insert(IEnumerable<UrlTrackerEntry> entries);
        void ResetCache();
        void SetConfiguration(UrlTrackerSettings? settings);
        Task WipeUrlTrackerTablesAsync();
    }

    public class PresetService : IPresetService
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly IContentService _contentService;
        private readonly IUrlTrackerConfigurationManager _urlTrackerConfigurationManager;
        private readonly IInterceptCache _interceptCache;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<PresetService> _logger;

        public PresetService(IScopeProvider scopeProvider,
                             IContentService contentService,
                             IUrlTrackerConfigurationManager urlTrackerConfigurationManager,
                             IInterceptCache interceptCache,
                             IMemoryCache memoryCache,
                             ILogger<PresetService> logger)
        {
            _scopeProvider = scopeProvider;
            _contentService = contentService;
            _urlTrackerConfigurationManager = urlTrackerConfigurationManager;
            _interceptCache = interceptCache;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task WipeUrlTrackerTablesAsync()
        {
            using var scope = _scopeProvider.CreateScope();
            await scope.Database.DeleteMany<UrlTrackerEntry>().ExecuteAsync().ConfigureAwait(false);
            scope.Complete();
        }

        public void EnsureContent()
        {
            WipeContent();

            /* Deze methode maakt de volgende content aan:
             *  - root
             *    - lorem
             *    - ipsum
             */
            var root = _contentService.Create("root", -1, "root");
            _contentService.SaveAndPublish(root);
            var content = _contentService.Create("lorem", root, "testPage");
            _contentService.SaveAndPublish(content);
            content = _contentService.Create("ipsum", root, "testPage");
            _contentService.SaveAndPublish(content);
        }

        public void WipeContent()
        {
            _contentService.EmptyRecycleBin(-1);

            var rootContent = _contentService.GetRootContent();
            foreach (var content in rootContent)
            {
                _contentService.Delete(content);
            }
        }

        public void Insert(IEnumerable<UrlTrackerEntry> entries)
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                scope.Database.InsertBulk(entries);
                scope.Complete();
            }

            foreach (var entry in entries)
            {
                _logger.LogDebug("Inserted entry: {entry}", entry);
            }
        }

        public void ResetCache()
        {
            _memoryCache.Remove(Core.Defaults.Cache.DomainKey);
            _memoryCache.Remove(Core.Defaults.Cache.RegexRedirectKey);
            _interceptCache.Clear();
        }

        public void SetConfiguration(UrlTrackerSettings? settings)
        {
            _urlTrackerConfigurationManager.Configuration = settings;
        }
    }
}