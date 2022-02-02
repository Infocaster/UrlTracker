using LightInject;
using Umbraco.Core;
using Umbraco.Core.Composing;
using UrlTracker.Core.Configuration;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Resources.Website.Maps;
using UrlTracker.Resources.Website.Preset;
using UrlTracker.Web;

namespace UrlTracker.Resources.Website.Composing
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run), ComposeAfter(typeof(WebComposer))]
    public class PresetComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            var container = composition.Concrete as ServiceContainer;

            composition.RegisterUnique<IPresetService, PresetService>();
            composition.RegisterUnique<IUrlTrackerConfigurationManager, UrlTrackerConfigurationManager>();

            container.Decorate<IConfiguration<UrlTrackerSettings>, DecoratorUrlTrackerConfigurationHijack>();

            composition.MapDefinitions()
                .Add<PresetMaps>();
        }
    }
}