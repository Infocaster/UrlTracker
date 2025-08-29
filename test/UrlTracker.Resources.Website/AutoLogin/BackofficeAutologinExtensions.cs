using Umbraco.Cms.Api.Management.Security;

namespace UrlTracker.Resources.Website.AutoLogin;

internal static class BackofficeAutologinExtensions
{
    public static IUmbracoBuilder AddBackofficeAutoLoginIfConfigured(this IUmbracoBuilder builder)
    {
        builder.Services.ConfigureOptions<BackofficeAutologinProviderOptions>();

        string? userEmail = builder.Config.GetValue<string>("Autologin:Backoffice:Email");
        if (string.IsNullOrWhiteSpace(userEmail)) return builder;

        builder.AddBackOfficeExternalLogins(logins =>
        {
            logins.AddBackOfficeLogin(authBuilder =>
            {
                authBuilder.AddRemoteScheme<AutoLoginOptions, BackofficeAutologinAuthenticationHandler>(BackOfficeAuthenticationBuilder.SchemeForBackOffice(AutoLoginOptions.AuthenticationScheme)!, "developer login", alOptions =>
                {
                    alOptions.CallbackPath = new PathString("/umbraco-auto-login");
                    alOptions.UserEmail = userEmail;
                });
            });
        });

        return builder;
    }
}
