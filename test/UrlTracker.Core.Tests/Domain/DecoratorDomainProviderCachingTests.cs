using System.Linq;
using Moq;
using NUnit.Framework;
using Umbraco.Core.Cache;
using UrlTracker.Core.Domain;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Resources.Testing;

namespace UrlTracker.Core.Tests.Domain
{
    public class DecoratorDomainProviderCachingTests : TestBase
    {
        private ObjectCacheAppCache _runtimeCache;
        private DecoratorDomainProviderCaching _testSubject;

        public override void SetUp()
        {
            _runtimeCache = new ObjectCacheAppCache();
            _testSubject = new DecoratorDomainProviderCaching(DomainProvider, _runtimeCache);
        }

        public override void TearDown()
        {
            _runtimeCache?.Dispose();
        }

        [TestCase(TestName = "GetDomains calls decoratee only once")]
        public void GetDomains_NormalFlow_GetsDomainsFromDecorateeOnlyOnce()
        {
            // arrange
            var output = DomainCollection.Create(Enumerable.Empty<Core.Domain.Models.Domain>());
            DomainProviderMock.Setup(obj => obj.GetDomains())
                .Returns(output);

            // act
            var result1 = _testSubject.GetDomains();
            var result2 = _testSubject.GetDomains();

            // assert
            DomainProviderMock.Verify(obj => obj.GetDomains(), Times.Once);
            Assert.That(result1, Is.SameAs(result2));
        }
    }
}
