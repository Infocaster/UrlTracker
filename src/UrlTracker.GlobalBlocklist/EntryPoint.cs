using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using UrlTracker.GlobalBlocklist.Context;
using UrlTracker.GlobalBlocklist.Filters;
using UrlTracker.GlobalBlocklist.Services;
using UrlTracker.Modules.Options;
using UrlTracker.Web;

namespace UrlTracker.GlobalBlocklist
{
    public static class EntryPoint
    {
        public static IUmbracoBuilder ComposeUrlTrackerGlobalDenyList(this IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<IDenyListContext, DenyListContext>();
            builder.Services.AddHostedService<DenyListPopulator>();

            builder.Services.AddHttpClient<RetrieveDenyListService>();
            builder.Services.AddSingleton<IRetrieveDenyListService>(services => services.GetRequiredService<RetrieveDenyListService>());
            builder.ClientErrorFilters()!
                .Append<GlobalDenyListFilter>();

            builder.Services.AddUrlTrackerModule("Global recommendation filter");

            return builder;
        }
    }
}
