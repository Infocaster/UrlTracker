using System.Web.Security;
using Moq;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Composing;
using Umbraco.Core.Dictionary;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.PublishedCache;
using Umbraco.Web.Security;
using Umbraco.Web.Security.Providers;
using UrlTracker.Resources.Testing;
using UrlTracker.Resources.Testing.Logging;
using UrlTracker.Resources.Testing.Mocks;

namespace UrlTracker.Web.Tests.Controllers
{
    public abstract class ControllerTestsBase : TestBase
    {
        protected ServiceContext ServiceContext { get; private set; }
        protected MembershipHelper MembershipHelper { get; private set; }
        protected UmbracoHelper UmbracoHelper { get; private set; }

        protected Mock<ICultureDictionary> CultureDictionaryMock { get; private set; }
        protected Mock<ICultureDictionaryFactory> CultureDictionaryFactoryMock { get; private set; }
        protected Mock<IPublishedContentQuery> PublishedContentQueryMock { get; private set; }
        protected Mock<IMemberService> MemberServiceMock { get; private set; }
        protected Mock<IPublishedMemberCache> MemberCacheMock { get; private set; }

        public override void SetUp()
        {
            SetupCultureDictionaries();
            SetupPublishedContentQuerying();
            SetupMembership();

            ServiceContext = ServiceContext.CreatePartial();
            UmbracoHelper = new UmbracoHelper(Mock.Of<IPublishedContent>(), Mock.Of<ITagQuery>(), CultureDictionaryFactoryMock.Object, Mock.Of<IUmbracoComponentRenderer>(), PublishedContentQueryMock.Object, MembershipHelper);
            SetupMapper();
        }

        protected virtual void SetupCultureDictionaries()
        {
            CultureDictionaryMock = new Mock<ICultureDictionary>();
            CultureDictionaryFactoryMock = new Mock<ICultureDictionaryFactory>();
            CultureDictionaryFactoryMock.Setup(obj => obj.CreateDictionary()).Returns(CultureDictionaryMock.Object);
        }

        protected virtual void SetupPublishedContentQuerying()
        {
            PublishedContentQueryMock = new Mock<IPublishedContentQuery>();
        }

        protected virtual void SetupMembership()
        {
            MemberServiceMock = new Mock<IMemberService>();
            var memberTypeService = Mock.Of<IMemberTypeService>();
            var membershipProvider = new MembersMembershipProvider(MemberServiceMock.Object, memberTypeService);

            MemberCacheMock = new Mock<IPublishedMemberCache>();
            MembershipHelper = new MembershipHelper(HttpContextMock.Context, MemberCacheMock.Object, membershipProvider, Mock.Of<RoleProvider>(), MemberServiceMock.Object, memberTypeService, Mock.Of<IUserService>(), Mock.Of<IPublicAccessService>(), AppCaches.NoCache, new ConsoleLogger());
        }

        protected virtual void SetupMapper()
        {
            // register the mapper in the current factory, because the authorized api controller uses an obsolete constructor which takes the mapper from the service locator
            var composition = new Composition(RegisterFactory.Create(), new TypeLoader(Mock.Of<IAppPolicyCache>(), string.Empty, Mock.Of<IProfilingLogger>()), Mock.Of<IProfilingLogger>(), Mock.Of<IRuntimeState>());
            composition.RegisterUnique(Mapper);
            Current.Factory = composition.CreateFactory();
        }

        public override void TearDown()
        {
            Current.Reset();
        }
    }
}
