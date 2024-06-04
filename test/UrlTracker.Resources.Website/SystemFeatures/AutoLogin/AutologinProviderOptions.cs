using System;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Api.Management.Security;

namespace UrlTracker.Resources.Website.SystemFeatures.AutoLogin;

public class AutologinProviderOptions
    : IConfigureNamedOptions<BackOfficeExternalLoginProviderOptions>
{
    public void Configure(string? name, BackOfficeExternalLoginProviderOptions options)
    {
        if (!string.Equals(name, "Umbraco." + AutoAuthenticationOptions.AuthenticationScheme, StringComparison.Ordinal))
        {
            return;
        }

        Configure(options);
    }

    public void Configure(BackOfficeExternalLoginProviderOptions options)
    {
        options.AutoLinkOptions = new ExternalSignInAutoLinkOptions(true);
    }
}
