using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;

namespace UrlTracker.Resources.Website.SystemFeatures.AutoLogin;

internal static class AutologinExtensions
{
    public static IUmbracoBuilder AddAutoLogin(this IUmbracoBuilder builder)
    {
        builder.Services.ConfigureOptions<AutologinProviderOptions>();

        builder.AddBackOfficeExternalLogins(logins =>
        {
            logins.AddBackOfficeLogin(authBuilder =>
            {
                authBuilder.AddRemoteScheme<AutoAuthenticationOptions, AutologinAuthenticationHandler>(authBuilder.SchemeForBackOffice(AutoAuthenticationOptions.AuthenticationScheme)!, "developer login", alOptions =>
                {
                    alOptions.CallbackPath = new PathString("/umbraco-auto-login");
                });
            });
        });

        return builder;
    }
}
