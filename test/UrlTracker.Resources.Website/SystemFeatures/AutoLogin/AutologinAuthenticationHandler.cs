using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Web.BackOffice.Security;
using Umbraco.Extensions;

namespace UrlTracker.Resources.Website.SystemFeatures.AutoLogin;

internal sealed class AutologinAuthenticationHandler
    : RemoteAuthenticationHandler<AutoAuthenticationOptions>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IBackOfficeUserManager _backOfficeUserManager;
    private readonly IBackOfficeSignInManager _backOfficeSignInManager;
    private readonly LinkGenerator _linkGenerator;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public AutologinAuthenticationHandler(
        IOptionsMonitor<AutoAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IHttpContextAccessor httpContextAccessor,
        IBackOfficeUserManager backOfficeUserManager,
        IBackOfficeSignInManager backOfficeSignInManager,
        LinkGenerator linkGenerator,
        IWebHostEnvironment webHostEnvironment)
        : base(options, logger, encoder, clock)
    {
        _httpContextAccessor = httpContextAccessor;
        _backOfficeUserManager = backOfficeUserManager;
        _backOfficeSignInManager = backOfficeSignInManager;
        _linkGenerator = linkGenerator;
        _webHostEnvironment = webHostEnvironment;
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        var httpContext = _httpContextAccessor.GetRequiredHttpContext();
        httpContext.Response.Redirect(Options.CallbackPath);

        return Task.CompletedTask;
    }

    protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
    {
        const string AuthenticationScheme = "Umbraco." + AutoAuthenticationOptions.AuthenticationScheme;
        var httpContext = _httpContextAccessor.GetRequiredHttpContext();

        if (!_webHostEnvironment.IsDevelopment()) return HandleRequestResult.NoResult();
        if (!httpContext.Request.IsLocal()) return HandleRequestResult.NoResult();

        var originalReturnUrl = httpContext.Request.Query["returnUrl"].FirstOrDefault() ?? "/umbraco";
        if (!originalReturnUrl.StartsWith("/umbraco", StringComparison.OrdinalIgnoreCase)) originalReturnUrl = "/umbraco";
        var returnUrl = originalReturnUrl;

        var identityUser = await _backOfficeUserManager.FindByIdAsync(Constants.System.RootString)
            ?? throw new InvalidOperationException("It's not possible to automatically log in without a root user");

        var properties = _backOfficeSignInManager.ConfigureExternalAuthenticationProperties(AuthenticationScheme, returnUrl, Constants.System.RootString);
        var principal = await _backOfficeSignInManager.CreateUserPrincipalAsync(identityUser);

        var ticket = new AuthenticationTicket(principal, properties, Constants.Security.BackOfficeExternalAuthenticationType);

        return HandleRequestResult.Success(ticket);
    }
}

internal sealed class AutoAuthenticationOptions
    : RemoteAuthenticationOptions
{
    public const string AuthenticationScheme = "AutoLogin";
}