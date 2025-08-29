using Microsoft.AspNetCore.Authentication;

namespace UrlTracker.Resources.Website.AutoLogin;

internal sealed class AutoLoginOptions
    : RemoteAuthenticationOptions
{
    public const string AuthenticationScheme = "AutoLogin";

    public string? UserEmail { get; set; }
}