using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Mapping;
using Umbraco.Core.Services;
using UrlTracker.Core;
using UrlTracker.Core.Abstractions;
using UrlTracker.Core.Caching;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Core.Database;
using UrlTracker.Core.Domain;
using UrlTracker.Core.Intercepting;
using UrlTracker.Core.Intercepting.Conversion;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Core.Intercepting.Preprocessing;
using UrlTracker.Core.Validation;
using UrlTracker.Resources.Testing.Mocks;
using UrlTracker.Resources.Testing.Objects;
using UrlTracker.Web.Abstractions;
using UrlTracker.Web.Compatibility;
using UrlTracker.Web.Configuration.Models;
using UrlTracker.Web.Processing;

namespace UrlTracker.Resources.Testing
{
    public abstract class TestBase
    {
        protected RegisterMock RegisterMock { get; set; }
        protected UmbracoContextFactoryAbstractionMock UmbracoContextFactoryAbstractionMock { get; set; }
        protected ScopeProviderMock ScopeProviderMock { get; set; }
        protected HttpContextMock HttpContextMock { get; set; }

        protected Mock<IUmbracoSettingsSection> UmbracoSettingsSectionMock { get; set; }
        protected IUmbracoSettingsSection UmbracoSettingsSection => UmbracoSettingsSectionMock.Object;
        protected Mock<IRequestHandlerSection> RequestHandlerSectionMock { get; set; }
        protected IRequestHandlerSection RequestHandlerSection => RequestHandlerSectionMock.Object;
        protected Mock<IAppSettingsAbstraction> AppSettingsAbstractionMock { get; set; }
        protected IAppSettingsAbstraction AppSettingsAbstraction => AppSettingsAbstractionMock.Object;
        protected Mock<IDomainProvider> DomainProviderMock { get; set; }
        protected IDomainProvider DomainProvider => DomainProviderMock.Object;
        protected Mock<IInterceptConverter> InterceptConverterMock { get; set; }
        protected IInterceptConverter InterceptConverter => InterceptConverterMock.Object;
        protected Mock<IInterceptPreprocessor> InterceptPreprocessorMock { get; set; }
        protected IInterceptPreprocessor InterceptPreprocessor => InterceptPreprocessorMock.Object;
        protected Mock<IDefaultInterceptContextFactory> DefaultInterceptContextFactoryMock { get; set; }
        protected IDefaultInterceptContextFactory DefaultInterceptContextFactory => DefaultInterceptContextFactoryMock.Object;
        protected Mock<IInterceptor> InterceptorMock { get; set; }
        protected IInterceptor Interceptor => InterceptorMock.Object;
        protected Mock<IClientErrorRepository> ClientErrorRepositoryMock { get; set; }
        protected IClientErrorRepository ClientErrorRepository => ClientErrorRepositoryMock.Object;
        protected Mock<IReferrerRepository> ReferrerRepositoryMock { get; set; }
        protected IReferrerRepository ReferrerRepository => ReferrerRepositoryMock.Object;
        protected Mock<IRedirectRepository> RedirectRepositoryMock { get; set; }
        protected IRedirectRepository RedirectRepository => RedirectRepositoryMock.Object;
        protected Mock<IValidationHelper> ValidationHelperMock { get; set; }
        protected IValidationHelper ValidationHelper => ValidationHelperMock.Object;
        protected Mock<IIntermediateInterceptService> IntermediateInterceptServiceMock { get; set; }
        protected IIntermediateInterceptService IntermediateInterceptService => IntermediateInterceptServiceMock.Object;
        protected Mock<IInterceptConverterCollection> InterceptConverterCollectionMock { get; set; }
        protected IInterceptConverterCollection InterceptConverterCollection => InterceptConverterCollectionMock.Object;
        protected Mock<IGlobalSettings> GlobalSettingsMock { get; set; }
        protected IGlobalSettings GlobalSettings => GlobalSettingsMock.Object;
        protected Mock<IRedirectService> RedirectServiceMock { get; set; }
        protected IRedirectService RedirectService => RedirectServiceMock.Object;
        protected Mock<IClientErrorService> ClientErrorServiceMock { get; set; }
        protected IClientErrorService ClientErrorService => ClientErrorServiceMock.Object;
        protected Mock<IRequestModelPatcher> RequestModelPatcherMock { get; set; }
        protected IRequestModelPatcher RequestModelPatcher => RequestModelPatcherMock.Object;
        protected Mock<IInterceptService> InterceptServiceMock { get; set; }
        protected IInterceptService InterceptService => InterceptServiceMock.Object;
        protected Mock<IResponseInterceptHandlerCollection> ResponseInterceptHandlerCollectionMock { get; set; }
        protected IResponseInterceptHandlerCollection ResponseInterceptHandlerCollection => ResponseInterceptHandlerCollectionMock.Object;
        protected Mock<ISpecificResponseInterceptHandler> ResponseInterceptHandlerMock { get; set; }
        protected ISpecificResponseInterceptHandler ResponseInterceptHandler => ResponseInterceptHandlerMock.Object;
        protected Mock<IRequestInterceptFilterCollection> RequestInterceptFilterCollectionMock { get; set; }
        protected IRequestInterceptFilterCollection RequestInterceptFilterCollection => RequestInterceptFilterCollectionMock.Object;
        protected Mock<IClientErrorFilterCollection> ClientErrorFilterCollectionMock { get; set; }
        protected IClientErrorFilterCollection ClientErrorFilterCollection => ClientErrorFilterCollectionMock.Object;
        protected Mock<IHttpContextAccessorAbstraction> HttpContextAccessorAbstractionMock { get; set; }
        protected IHttpContextAccessorAbstraction HttpContextAccessorAbstraction => HttpContextAccessorAbstractionMock.Object;
        protected Mock<ILocalizationService> LocalizationServiceMock { get; set; }
        protected ILocalizationService LocalizationService => LocalizationServiceMock.Object;
        protected Mock<ICompleteRequestAbstraction> CompleteRequestAbstractionMock { get; set; }
        protected ICompleteRequestAbstraction CompleteRequestAbstraction => CompleteRequestAbstractionMock.Object;
        protected Mock<IRequestInterceptFilter> RequestInterceptFilterMock { get; set; }
        protected IRequestInterceptFilter RequestInterceptFilter => RequestInterceptFilterMock.Object;
        protected Mock<IInterceptCache> InterceptCacheMock { get; set; }
        protected IInterceptCache InterceptCache => InterceptCacheMock.Object;
        protected Mock<IRuntimeState> RuntimeStateMock { get; set; }
        protected IRuntimeState RuntimeState => RuntimeStateMock.Object;



        protected TestConfiguration<UrlTrackerSettings> UrlTrackerSettings { get; set; }
        protected TestConfiguration<ReservedPathSettings> ReservedPathSettings { get; set; }
        protected UmbracoMapper Mapper { get; set; }
        protected DefaultInterceptContext DefaultInterceptContext { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUpBase()
        {
            UrlTrackerSettings = new TestConfiguration<UrlTrackerSettings>();
            ReservedPathSettings = new TestConfiguration<ReservedPathSettings>();

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
            UrlTrackerSettings.Value = null;
            ReservedPathSettings.Value = null;

            RequestHandlerSectionMock = new Mock<IRequestHandlerSection>();
            UmbracoSettingsSectionMock = new Mock<IUmbracoSettingsSection>();
            UmbracoSettingsSectionMock.Setup(obj => obj.RequestHandler).Returns(RequestHandlerSection);

            RegisterMock = new RegisterMock();
            UmbracoContextFactoryAbstractionMock = new UmbracoContextFactoryAbstractionMock();
            AppSettingsAbstractionMock = new Mock<IAppSettingsAbstraction>();
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
            GlobalSettingsMock = new Mock<IGlobalSettings>();
            RedirectServiceMock = new Mock<IRedirectService>();
            ClientErrorServiceMock = new Mock<IClientErrorService>();
            ScopeProviderMock = new ScopeProviderMock();
            RequestModelPatcherMock = new Mock<IRequestModelPatcher>();
            InterceptServiceMock = new Mock<IInterceptService>();
            ResponseInterceptHandlerCollectionMock = new Mock<IResponseInterceptHandlerCollection>();
            ResponseInterceptHandlerMock = new Mock<ISpecificResponseInterceptHandler>();
            RequestInterceptFilterCollectionMock = new Mock<IRequestInterceptFilterCollection>();
            ClientErrorFilterCollectionMock = new Mock<IClientErrorFilterCollection>();
            HttpContextAccessorAbstractionMock = new Mock<IHttpContextAccessorAbstraction>();
            LocalizationServiceMock = new Mock<ILocalizationService>();
            CompleteRequestAbstractionMock = new Mock<ICompleteRequestAbstraction>();
            RequestInterceptFilterMock = new Mock<IRequestInterceptFilter>();
            InterceptCacheMock = new Mock<IInterceptCache>();
            RuntimeStateMock = new Mock<IRuntimeState>();

            HttpContextMock = CreateHttpContextMock();

            Mapper = new UmbracoMapper(new MapDefinitionCollection(CreateMappers()));

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

        protected static TestMapDefinition<TIn, TOut> CreateTestMap<TIn, TOut>(TOut value = default)
            => new TestMapDefinition<TIn, TOut>() { To = value };

        protected virtual HttpContextMock CreateHttpContextMock()
            => new HttpContextMock();
    }
}
