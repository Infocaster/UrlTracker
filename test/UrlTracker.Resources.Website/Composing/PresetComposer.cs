using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using UrlTracker.Resources.Website.Maps;
using UrlTracker.Resources.Website.Preset;

namespace UrlTracker.Resources.Website.Composing
{
    public class PresetComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<IPresetService, PresetService>();
            builder.Services.AddSingleton<IUrlTrackerConfigurationManager, UrlTrackerConfigurationManager>();

            builder.MapDefinitions()
                .Add<PresetMaps>();
        }
    }
}