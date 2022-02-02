using System.Collections.Generic;
using System.Threading.Tasks;
using Umbraco.Core.Logging;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Core.Database.Models;

namespace UrlTracker.Resources.Website.Preset
{
    public interface IPresetService
    {
        void EnsureContent();
        void Insert(IEnumerable<UrlTrackerEntry> entries);
        void SetConfiguration(UrlTrackerSettings settings);
        Task WipeUrlTrackerTablesAsync();
    }

    public class PresetService : IPresetService
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly IContentService _contentService;
        private readonly IUrlTrackerConfigurationManager _urlTrackerConfigurationManager;
        private readonly ILogger _logger;

        public PresetService(IScopeProvider scopeProvider, IContentService contentService, IUrlTrackerConfigurationManager urlTrackerConfigurationManager, ILogger logger)
        {
            _scopeProvider = scopeProvider;
            _contentService = contentService;
            _urlTrackerConfigurationManager = urlTrackerConfigurationManager;
            _logger = logger;
        }

        public async Task WipeUrlTrackerTablesAsync()
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                await scope.Database.DeleteMany<UrlTrackerEntry>().ExecuteAsync().ConfigureAwait(false);
                scope.Complete();
            }
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
                _logger.Debug<PresetService>("Inserted entry: {entry}", entry);
            }
        }

        public void SetConfiguration(UrlTrackerSettings settings)
        {
            _urlTrackerConfigurationManager.Configuration = settings;
        }
    }
}