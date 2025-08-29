using Umbraco.Cms.Core.Composing;

namespace UrlTracker.Resources.Website.AutoLogin;

public class AutoLoginComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
        => builder
            .AddBackofficeAutoLoginIfConfigured();
}
