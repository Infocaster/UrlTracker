using System.Diagnostics.CodeAnalysis;
using Umbraco.Core.Cache;
using Umbraco.Core.Composing;
using Umbraco.Core.Services.Implement;

namespace UrlTracker.Core.Components
{
    [ExcludeFromCodeCoverage]
    public class ContentChangeHandlerComponent
        : IComponent
    {
        private readonly IAppPolicyCache _runtimeCache;

        public ContentChangeHandlerComponent(AppCaches appCaches)
        {
            _runtimeCache = appCaches.RuntimeCache;
        }

        public void Initialize()
        {
            DomainService.Deleted += DomainService_Deleted;
            DomainService.Saved += DomainService_Saved;
        }

        public void Terminate()
        {
            DomainService.Deleted -= DomainService_Deleted;
            DomainService.Saved -= DomainService_Saved;
        }

        private void DomainService_Deleted(Umbraco.Core.Services.IDomainService sender, Umbraco.Core.Events.DeleteEventArgs<Umbraco.Core.Models.IDomain> e)
        {
            _runtimeCache.Clear(Defaults.Cache.DomainKey);
        }

        private void DomainService_Saved(Umbraco.Core.Services.IDomainService sender, Umbraco.Core.Events.SaveEventArgs<Umbraco.Core.Models.IDomain> e)
        {
            _runtimeCache.Clear(Defaults.Cache.DomainKey);
        }
    }
}
