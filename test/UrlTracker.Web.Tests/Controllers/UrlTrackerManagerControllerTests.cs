using System.Collections.Generic;
using Moq;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Configuration;
using Umbraco.Core.Logging;
using Umbraco.Core.Mapping;
using Umbraco.Core.Persistence;
using Umbraco.Web;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Models;
using UrlTracker.Resources.Testing.Mocks;
using UrlTracker.Web.Controllers;
using UrlTracker.Web.Controllers.Models;

namespace UrlTracker.Web.Tests.Controllers
{
    public partial class UrlTrackerManagerControllerTests : ControllerTestsBase
    {
        private UrlTrackerManagerController _testSubject;

        protected override ICollection<IMapDefinition> CreateMappers()
        {
            return new IMapDefinition[]
            {
                CreateTestMap<UrlTrackerSettings, GetSettingsResponse>(new GetSettingsResponse()),
                CreateTestMap<Domain, GetLanguagesFromNodeResponseLanguage>(new GetLanguagesFromNodeResponseLanguage
                {
                    CultureName = "English (USA)",
                    Id = 1,
                    IsoCode = "en-us"
                }),
                CreateTestMap<AddRedirectRequest, Redirect>(new Redirect()),
                CreateTestMap<UpdateRedirectRequest, Redirect>(new Redirect()),
                CreateTestMap<RedirectCollection, GetRedirectsResponse>(new GetRedirectsResponse())
            };
        }

        public override void SetUp()
        {
            base.SetUp();
            UrlTrackerSettings.Value = new UrlTrackerSettings(true, true, true, true, true, true, 5000, true, 60 * 48, true);
            _testSubject = new UrlTrackerManagerController(Mock.Of<IGlobalSettings>(),
                                                           Mock.Of<IUmbracoContextAccessor>(),
                                                           Mock.Of<ISqlContext>(),
                                                           ServiceContext,
                                                           AppCaches.NoCache,
                                                           Mock.Of<IProfilingLogger>(),
                                                           Mock.Of<IRuntimeState>(),
                                                           UmbracoHelper,
                                                           UrlTrackerSettings,
                                                           DomainProvider,
                                                           RedirectService,
                                                           ClientErrorService,
                                                           LegacyService,
                                                           ScopeProviderMock.Provider,
                                                           RequestModelPatcher);
        }
    }
}
