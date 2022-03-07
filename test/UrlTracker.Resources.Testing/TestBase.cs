using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using UrlTracker.Core;
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
using UrlTracker.Web.Abstraction;
using UrlTracker.Web.Compatibility;
using UrlTracker.Web.Configuration;
using UrlTracker.Web.Processing;

namespace UrlTracker.Resources.Testing
{
    public abstract class TestBase
    {
        protected UmbracoContextFactoryAbstractionMock UmbracoContextFactoryAbstractionMock { get; set; }
        protected ScopeProviderMock ScopeProviderMock { get; private set; }
        protected HttpContextMock HttpContextMock { get; set; }
        protected IOptions<GlobalSettings> GlobalSettings { get; set; }

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
        protected Mock<IRedirectRepository> RedirectRepositoryMock { get; set; }
        protected IRedirectRepository RedirectRepository => RedirectRepositoryMock.Object;
        protected Mock<IValidationHelper> ValidationHelperMock { get; set; }
        protected IValidationHelper ValidationHelper => ValidationHelperMock.Object;
        protected Mock<IIntermediateInterceptService> IntermediateInterceptServiceMock { get; set; }
        protected IIntermediateInterceptService IntermediateInterceptService => IntermediateInterceptServiceMock.Object;
        protected Mock<IInterceptConverterCollection> InterceptConverterCollectionMock { get; set; }
        protected IInterceptConverterCollection InterceptConverterCollection => InterceptConverterCollectionMock.Object;
        protected Mock<IRedirectService> RedirectServiceMock { get; set; }
        protected IRedirectService RedirectService => RedirectServiceMock.Object;
        protected Mock<IClientErrorService> ClientErrorServiceMock { get; set; }
        protected IClientErrorService ClientErrorService => ClientErrorServiceMock.Object;
        protected Mock<ILegacyService> LegacyServiceMock { get; set; }
        protected ILegacyService LegacyService => LegacyServiceMock.Object;
        protected Mock<IRequestModelPatcher> RequestModelPatcherMock { get; set; }
        protected IRequestModelPatcher RequestModelPatcher => RequestModelPatcherMock.Object;
        protected Mock<IInterceptService> InterceptServiceMock { get; set; }
        protected IInterceptService InterceptService => InterceptServiceMock.Object;
        protected Mock<IResponseInterceptHandlerCollection> ResponseInterceptHandlerCollectionMock { get; set; }
        protected IResponseInterceptHandlerCollection ResponseInterceptHandlerCollection => ResponseInterceptHandlerCollectionMock.Object;
        protected Mock<IResponseInterceptHandler> ResponseInterceptHandlerMock { get; set; }
        protected IResponseInterceptHandler ResponseInterceptHandler => ResponseInterceptHandlerMock.Object;
        protected Mock<IRequestInterceptFilterCollection> RequestInterceptFilterCollectionMock { get; set; }
        protected IRequestInterceptFilterCollection RequestInterceptFilterCollection => RequestInterceptFilterCollectionMock.Object;
        protected Mock<IClientErrorFilterCollection> ClientErrorFilterCollectionMock { get; set; }
        protected IClientErrorFilterCollection ClientErrorFilterCollection => ClientErrorFilterCollectionMock.Object;
        protected Mock<ILocalizationService> LocalizationServiceMock { get; set; }
        protected ILocalizationService LocalizationService => LocalizationServiceMock.Object;
        protected Mock<IRequestInterceptFilter> RequestInterceptFilterMock { get; set; }
        protected IRequestInterceptFilter RequestInterceptFilter => RequestInterceptFilterMock.Object;
        protected Mock<IRequestAbstraction> RequestAbstractionMock { get; set; }
        protected IRequestAbstraction RequestAbstraction => RequestAbstractionMock.Object;
        protected Mock<IResponseAbstraction> ResponseAbstractionMock { get; set; }
        protected IResponseAbstraction ResponseAbstraction => ResponseAbstractionMock.Object;



        protected IOptions<UrlTrackerSettings> UrlTrackerSettings { get; set; }
        protected Mock<IReservedPathSettingsProvider> ReservedPathSettingsProviderMock { get; set; }
        protected IReservedPathSettingsProvider ReservedPathSettingsProvider => ReservedPathSettingsProviderMock.Object;
        protected IUmbracoMapper Mapper { get; set; }
        protected DefaultInterceptContext DefaultInterceptContext { get; set; }

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
            UrlTrackerSettings = Options.Create<UrlTrackerSettings>(new UrlTrackerSettings());
            GlobalSettings = Options.Create<GlobalSettings>(new GlobalSettings());

            ReservedPathSettingsProviderMock = new Mock<IReservedPathSettingsProvider>();
            UmbracoContextFactoryAbstractionMock = new UmbracoContextFactoryAbstractionMock();
            DomainProviderMock = new Mock<IDomainProvider>();
            InterceptConverterMock = new Mock<IInterceptConverter>();
            DefaultInterceptContext = new DefaultInterceptContext();
            InterceptPreprocessorMock = new Mock<IInterceptPreprocessor>();
            DefaultInterceptContextFactoryMock = new Mock<IDefaultInterceptContextFactory>();
            InterceptorMock = new Mock<IInterceptor>();
            ClientErrorRepositoryMock = new Mock<IClientErrorRepository>();
            RedirectRepositoryMock = new Mock<IRedirectRepository>();
            ValidationHelperMock = new Mock<IValidationHelper>();
            IntermediateInterceptServiceMock = new Mock<IIntermediateInterceptService>();
            InterceptConverterCollectionMock = new Mock<IInterceptConverterCollection>();
            RedirectServiceMock = new Mock<IRedirectService>();
            ClientErrorServiceMock = new Mock<IClientErrorService>();
            LegacyServiceMock = new Mock<ILegacyService>();
            ScopeProviderMock = new ScopeProviderMock();
            RequestModelPatcherMock = new Mock<IRequestModelPatcher>();
            InterceptServiceMock = new Mock<IInterceptService>();
            ResponseInterceptHandlerCollectionMock = new Mock<IResponseInterceptHandlerCollection>();
            ResponseInterceptHandlerMock = new Mock<IResponseInterceptHandler>();
            RequestInterceptFilterCollectionMock = new Mock<IRequestInterceptFilterCollection>();
            ClientErrorFilterCollectionMock = new Mock<IClientErrorFilterCollection>();
            LocalizationServiceMock = new Mock<ILocalizationService>();
            RequestInterceptFilterMock = new Mock<IRequestInterceptFilter>();
            RequestAbstractionMock = new Mock<IRequestAbstraction>();
            ResponseAbstractionMock = new Mock<IResponseAbstraction>();

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

        protected TestMapDefinition<TIn, TOut> CreateTestMap<TIn, TOut>(TOut? value = default)
            => new TestMapDefinition<TIn, TOut>() { To = value };

        protected virtual HttpContextMock CreateHttpContextMock()
            => new HttpContextMock();
    }
}
