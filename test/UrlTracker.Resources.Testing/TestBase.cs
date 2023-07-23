using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Scoping;
using UrlTracker.Backoffice.UI.Compatibility;
using UrlTracker.Core;
using UrlTracker.Core.Caching.Memory;
using UrlTracker.Core.Caching.Memory.Options;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Core.Database;
using UrlTracker.Core.Domain;
using UrlTracker.Core.Intercepting;
using UrlTracker.Core.Intercepting.Conversion;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Core.Intercepting.Preprocessing;
using UrlTracker.Core.Map;
using UrlTracker.Core.Validation;
using UrlTracker.Middleware.Options;
using UrlTracker.Modules.Options;
using UrlTracker.Resources.Testing.Mocks;
using UrlTracker.Resources.Testing.Objects;
using UrlTracker.Web.Abstraction;
using UrlTracker.Web.Configuration;
using UrlTracker.Web.Processing;

namespace UrlTracker.Resources.Testing
{
    public abstract class TestBase
    {
        protected UmbracoContextFactoryAbstractionMock UmbracoContextFactoryAbstractionMock { get; set; } = null!;
        protected ScopeProviderMock ScopeProviderMock { get; set; } = null!;
        protected HttpContextMock HttpContextMock { get; set; } = null!;
        protected IOptions<GlobalSettings> GlobalSettings { get; set; } = null!;

        protected Mock<IOptionsMonitor<RequestHandlerSettings>> RequestHandlerSettingsMock { get; set; } = null!;
        protected IOptionsMonitor<RequestHandlerSettings> RequestHandlerSettings => RequestHandlerSettingsMock.Object;
        protected Mock<IOptionsMonitor<UrlTrackerSettings>> UrlTrackerSettingsMock { get; set; } = null!;
        protected IOptionsMonitor<UrlTrackerSettings> UrlTrackerSettings => UrlTrackerSettingsMock.Object;
        protected Mock<IOptionsSnapshot<UrlTrackerLegacyOptions>> UrlTrackerLegacyOptionsMock { get; set; } = null!;
        protected IOptionsSnapshot<UrlTrackerLegacyOptions> UrlTrackerLegacyOptions => UrlTrackerLegacyOptionsMock.Object;
        protected Mock<IOptionsMonitor<UrlTrackerPipelineOptions>> UrlTrackerPipelineOptionsMock { get; set; } = null!;
        protected IOptionsMonitor<UrlTrackerPipelineOptions> UrlTrackerPipelineOptions => UrlTrackerPipelineOptionsMock.Object;
        protected Mock<IDomainProvider> DomainProviderMock { get; set; } = null!;
        protected IDomainProvider DomainProvider => DomainProviderMock.Object;
        protected Mock<IInterceptConverter> InterceptConverterMock { get; set; } = null!;
        protected IInterceptConverter InterceptConverter => InterceptConverterMock.Object;
        protected Mock<IInterceptPreprocessor> InterceptPreprocessorMock { get; set; } = null!;
        protected IInterceptPreprocessor InterceptPreprocessor => InterceptPreprocessorMock.Object;
        protected Mock<IDefaultInterceptContextFactory> DefaultInterceptContextFactoryMock { get; set; } = null!;
        protected IDefaultInterceptContextFactory DefaultInterceptContextFactory => DefaultInterceptContextFactoryMock.Object;
        protected Mock<IInterceptor> InterceptorMock { get; set; } = null!;
        protected IInterceptor Interceptor => InterceptorMock.Object;
        protected Mock<IClientErrorRepository> ClientErrorRepositoryMock { get; set; } = null!;
        protected IClientErrorRepository ClientErrorRepository => ClientErrorRepositoryMock.Object;
        protected Mock<IReferrerRepository> ReferrerRepositoryMock { get; set; } = null!;
        protected IReferrerRepository ReferrerRepository => ReferrerRepositoryMock.Object;
        protected Mock<IRedirectRepository> RedirectRepositoryMock { get; set; } = null!;
        protected IRedirectRepository RedirectRepository => RedirectRepositoryMock.Object;
        protected Mock<IValidationHelper> ValidationHelperMock { get; set; } = null!;
        protected IValidationHelper ValidationHelper => ValidationHelperMock.Object;
        protected Mock<IIntermediateInterceptService> IntermediateInterceptServiceMock { get; set; } = null!;
        protected IIntermediateInterceptService IntermediateInterceptService => IntermediateInterceptServiceMock.Object;
        protected Mock<IInterceptConverterCollection> InterceptConverterCollectionMock { get; set; } = null!;
        protected IInterceptConverterCollection InterceptConverterCollection => InterceptConverterCollectionMock.Object;
        protected Mock<IRedirectService> RedirectServiceMock { get; set; } = null!;
        protected IRedirectService RedirectService => RedirectServiceMock.Object;
        protected Mock<IClientErrorService> ClientErrorServiceMock { get; set; } = null!;
        protected IClientErrorService ClientErrorService => ClientErrorServiceMock.Object;
        protected Mock<IRequestModelPatcher> RequestModelPatcherMock { get; set; } = null!;
        protected IRequestModelPatcher RequestModelPatcher => RequestModelPatcherMock.Object;
        protected Mock<IInterceptService> InterceptServiceMock { get; set; } = null!;
        protected IInterceptService InterceptService => InterceptServiceMock.Object;
        protected Mock<IResponseInterceptHandlerCollection> ResponseInterceptHandlerCollectionMock { get; set; } = null!;
        protected IResponseInterceptHandlerCollection ResponseInterceptHandlerCollection => ResponseInterceptHandlerCollectionMock.Object;
        protected Mock<ISpecificResponseInterceptHandler> ResponseInterceptHandlerMock { get; set; } = null!;
        protected ISpecificResponseInterceptHandler ResponseInterceptHandler => ResponseInterceptHandlerMock.Object;
        protected Mock<IRequestInterceptFilterCollection> RequestInterceptFilterCollectionMock { get; set; } = null!;
        protected IRequestInterceptFilterCollection RequestInterceptFilterCollection => RequestInterceptFilterCollectionMock.Object;
        protected Mock<IClientErrorFilterCollection> ClientErrorFilterCollectionMock { get; set; } = null!;
        protected IClientErrorFilterCollection ClientErrorFilterCollection => ClientErrorFilterCollectionMock.Object;
        protected Mock<ILocalizationService> LocalizationServiceMock { get; set; } = null!;
        protected ILocalizationService LocalizationService => LocalizationServiceMock.Object;
        protected Mock<IRequestInterceptFilter> RequestInterceptFilterMock { get; set; } = null!;
        protected IRequestInterceptFilter RequestInterceptFilter => RequestInterceptFilterMock.Object;
        protected Mock<IRequestAbstraction> RequestAbstractionMock { get; set; } = null!;
        protected IRequestAbstraction RequestAbstraction => RequestAbstractionMock.Object;
        protected Mock<IResponseAbstraction> ResponseAbstractionMock { get; set; } = null!;
        protected IResponseAbstraction ResponseAbstraction => ResponseAbstractionMock.Object;
        protected Mock<IInterceptCache> InterceptCacheMock { get; set; } = null!;
        protected IInterceptCache InterceptCache => InterceptCacheMock.Object;
        protected Mock<IRuntimeState> RuntimeStateMock { get; set; } = null!;
        protected IRuntimeState RuntimeState => RuntimeStateMock.Object;
        protected Mock<IStrategyMapCollection> StrategyMapCollectionMock { get; set; } = null!;
        protected IStrategyMapCollection StrategyMapCollection => StrategyMapCollectionMock.Object;
        protected Mock<IRedactionScoreService> RedactionScoreServiceMock { get; set; } = null!;
        protected IRedactionScoreService RedactionScoreService => RedactionScoreServiceMock.Object;


        protected IOptions<UrlTrackerMemoryCacheOptions> UrlTrackerMemoryCacheOptions { get; set; } = null!;
        protected Mock<IReservedPathSettingsProvider> ReservedPathSettingsProviderMock { get; set; } = null!;
        protected IReservedPathSettingsProvider ReservedPathSettingsProvider => ReservedPathSettingsProviderMock.Object;
        protected IUmbracoMapper Mapper { get; set; } = null!;
        protected DefaultInterceptContext DefaultInterceptContext { get; set; } = null!;

        [OneTimeSetUp]
        public void OneTimeSetUpBase()
        {
            OneTimeSetUp();
        }

        public virtual void OneTimeSetUp()
        { }

        [OneTimeTearDown]
        public void OneTimeTearDownBase()
        {
            OneTimeTearDown();
        }

        public virtual void OneTimeTearDown()
        { }

        [SetUp]
        public void SetUpBase()
        {
            UrlTrackerSettingsMock = new Mock<IOptionsMonitor<UrlTrackerSettings>>();
            UrlTrackerSettingsMock.SetupGet(obj => obj.CurrentValue).Returns(new UrlTrackerSettings());
            UrlTrackerPipelineOptionsMock = new Mock<IOptionsMonitor<UrlTrackerPipelineOptions>>();
            UrlTrackerPipelineOptionsMock.SetupGet(obj => obj.CurrentValue).Returns(new UrlTrackerPipelineOptions());
            UrlTrackerLegacyOptionsMock = new Mock<IOptionsSnapshot<UrlTrackerLegacyOptions>>();
            UrlTrackerLegacyOptionsMock.SetupGet(obj => obj.Value).Returns(new UrlTrackerLegacyOptions());
            UrlTrackerMemoryCacheOptions = Options.Create<UrlTrackerMemoryCacheOptions>(new UrlTrackerMemoryCacheOptions());
            GlobalSettings = Options.Create<GlobalSettings>(new GlobalSettings());

            RequestHandlerSettingsMock = new Mock<IOptionsMonitor<RequestHandlerSettings>>();
            ReservedPathSettingsProviderMock = new Mock<IReservedPathSettingsProvider>();
            UmbracoContextFactoryAbstractionMock = new UmbracoContextFactoryAbstractionMock();
            DomainProviderMock = new Mock<IDomainProvider>();
            InterceptConverterMock = new Mock<IInterceptConverter>();
            DefaultInterceptContext = new DefaultInterceptContext();
            InterceptPreprocessorMock = new Mock<IInterceptPreprocessor>();
            DefaultInterceptContextFactoryMock = new Mock<IDefaultInterceptContextFactory>();
            InterceptorMock = new Mock<IInterceptor>();
            ClientErrorRepositoryMock = new Mock<IClientErrorRepository>();
            ReferrerRepositoryMock = new Mock<IReferrerRepository>();
            RedirectRepositoryMock = new Mock<IRedirectRepository>();
            ValidationHelperMock = new Mock<IValidationHelper>();
            IntermediateInterceptServiceMock = new Mock<IIntermediateInterceptService>();
            InterceptConverterCollectionMock = new Mock<IInterceptConverterCollection>();
            RedirectServiceMock = new Mock<IRedirectService>();
            ClientErrorServiceMock = new Mock<IClientErrorService>();
            ScopeProviderMock = new ScopeProviderMock();
            RequestModelPatcherMock = new Mock<IRequestModelPatcher>();
            InterceptServiceMock = new Mock<IInterceptService>();
            ResponseInterceptHandlerCollectionMock = new Mock<IResponseInterceptHandlerCollection>();
            ResponseInterceptHandlerMock = new Mock<ISpecificResponseInterceptHandler>();
            RequestInterceptFilterCollectionMock = new Mock<IRequestInterceptFilterCollection>();
            ClientErrorFilterCollectionMock = new Mock<IClientErrorFilterCollection>();
            LocalizationServiceMock = new Mock<ILocalizationService>();
            RequestInterceptFilterMock = new Mock<IRequestInterceptFilter>();
            RequestAbstractionMock = new Mock<IRequestAbstraction>();
            ResponseAbstractionMock = new Mock<IResponseAbstraction>();
            InterceptCacheMock = new Mock<IInterceptCache>();
            RuntimeStateMock = new Mock<IRuntimeState>();
            StrategyMapCollectionMock = new Mock<IStrategyMapCollection>();
            RedactionScoreServiceMock = new Mock<IRedactionScoreService>();

            HttpContextMock = CreateHttpContextMock();

            Mapper = new UmbracoMapper(new MapDefinitionCollection(CreateMappers), Mock.Of<IScopeProvider>());

            SetUp();
        }

        public virtual void SetUp()
        { }

        [TearDown]
        public void TearDownBase()
        {
            TearDown();
        }

        public virtual void TearDown()
        { }

        protected virtual ICollection<IMapDefinition> CreateMappers()
        {
            return Array.Empty<IMapDefinition>();
        }

        protected static TestMapDefinition<TIn, TOut> CreateTestMap<TIn, TOut>(TOut? value = default)
            => new() { To = value };

        protected virtual HttpContextMock CreateHttpContextMock()
            => new();
    }
}
