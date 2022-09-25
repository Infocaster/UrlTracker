using Umbraco.Cms.Core.Mapping;
using UrlTracker.Backoffice.UI.Controllers;
using UrlTracker.Backoffice.UI.Controllers.Models;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Models;
using UrlTracker.Resources.Testing;

namespace UrlTracker.Web.Tests.Controllers
{
    public partial class UrlTrackerManagerControllerTests : TestBase
    {
        private UrlTrackerManagerController _testSubject = null!;

        protected override ICollection<IMapDefinition> CreateMappers()
        {
            return new IMapDefinition[]
            {
                CreateTestMap<UrlTrackerSettings, GetSettingsResponse>(new GetSettingsResponse()),
                CreateTestMap<Domain, GetLanguagesFromNodeResponseLanguage>(new GetLanguagesFromNodeResponseLanguage("en-us", "English (USA)")
                {
                    Id = 1
                }),
                CreateTestMap<AddRedirectRequest, Redirect>(new Redirect()),
                CreateTestMap<UpdateRedirectRequest, Redirect>(new Redirect()),
                CreateTestMap<RedirectCollection, GetRedirectsResponse>(new GetRedirectsResponse(new List<RedirectViewModel>(), 0))
            };
        }

        public override void SetUp()
        {
            base.SetUp();
            UrlTrackerLegacyOptions.Value.AppendPortNumber = true;
            UrlTrackerLegacyOptions.Value.IsDisabled = true;
            UrlTrackerLegacyOptions.Value.EnableLogging = true;
            _testSubject = new UrlTrackerManagerController(UrlTrackerLegacyOptions,
                                                           DomainProvider,
                                                           RedirectService,
                                                           ClientErrorService,
                                                           ScopeProviderMock!.Provider,
                                                           RequestModelPatcher,
                                                           Mapper!,
                                                           UmbracoContextFactoryAbstractionMock.UmbracoContextFactory);
        }
    }
}
