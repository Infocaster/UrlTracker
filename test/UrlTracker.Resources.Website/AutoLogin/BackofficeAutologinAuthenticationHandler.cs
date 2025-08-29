using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Web.Common.Security;

namespace UrlTracker.Resources.Website.AutoLogin;

internal sealed class BackofficeAutologinAuthenticationHandler(
    IOptionsMonitor<AutoLoginOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IHttpContextAccessor httpContextAccessor,
    IBackOfficeUserManager backOfficeUserManager,
    IBackOfficeSignInManager backOfficeSignInManager,
    IWebHostEnvironment webHostEnvironment)
        : RemoteAuthenticationHandler<AutoLoginOptions>(options, logger, encoder)
{
    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        HttpContext httpContext = httpContextAccessor.GetRequiredHttpContext();
        httpContext.Response.Redirect(Options.CallbackPath);

        return Task.CompletedTask;
    }

    protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
    {
        const string AuthenticationScheme = "Umbraco." + AutoLoginOptions.AuthenticationScheme;
        HttpContext httpContext = httpContextAccessor.GetRequiredHttpContext();

        if (!webHostEnvironment.IsDevelopment()) return HandleRequestResult.NoResult();
        if (!httpContext.Request.IsLocal()) return HandleRequestResult.NoResult();

        string originalReturnUrl = httpContext.Request.Query["returnUrl"].FirstOrDefault() ?? "/umbraco";
        if (!originalReturnUrl.StartsWith("/umbraco", StringComparison.OrdinalIgnoreCase)) originalReturnUrl = "/umbraco";
        string returnUrl = originalReturnUrl;

        if (string.IsNullOrWhiteSpace(Options.UserEmail))
            throw new InvalidOperationException("Unable to log in with auto login, because no user email has been specified in config");

        BackOfficeIdentityUser identityUser = await backOfficeUserManager.FindByEmailAsync(Options.UserEmail)
            ?? throw new InvalidOperationException("The user with the configured email address could not be found");

        AuthenticationProperties properties = backOfficeSignInManager.ConfigureExternalAuthenticationProperties(AuthenticationScheme, returnUrl, Constants.System.RootString);
        System.Security.Claims.ClaimsPrincipal principal = await backOfficeSignInManager.CreateUserPrincipalAsync(identityUser);

        AuthenticationTicket ticket = new(principal, properties, Constants.Security.BackOfficeExternalAuthenticationType);

        return HandleRequestResult.Success(ticket);
    }
}
