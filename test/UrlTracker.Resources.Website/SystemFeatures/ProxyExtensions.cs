using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UrlTracker.Resources.Website.SystemFeatures;

public static class ProxyExtensions
{
    public static IServiceCollection ConfigureForwardedHeaders(this IServiceCollection services, IConfiguration config)
    {
        return services.Configure<ForwardedHeadersOptions>(options =>
        {
            // We are interested in all the headers:
            //    we pretend as if the request came straight from the client, rather than the proxy
            options.ForwardedHeaders = ForwardedHeaders.All;

            var forwardedForHeaderName = config.GetValue<string?>("ForwardedForHeaderName", null);
            if (forwardedForHeaderName is not null)
            {
                options.ForwardedForHeaderName = forwardedForHeaderName;
            }

            options.RequireHeaderSymmetry = false;

            // Fix according to stackoverflow: https://stackoverflow.com/a/43878365
            //    Other fix would be to explicitly define our known networks and proxies, that might be more safe than clearing them.
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });
    }
}
