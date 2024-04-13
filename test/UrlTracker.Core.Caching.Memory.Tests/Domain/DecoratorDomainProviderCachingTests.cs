using System.Linq;
using Moq;
using NUnit.Framework;
using Umbraco.Cms.Core.Cache;
using UrlTracker.Core.Caching.Memory.Domain;
using UrlTracker.Core.Domain;
using UrlTracker.Core.Domain.Models;

namespace UrlTracker.Core.Caching.Memory.Tests.Domain
{
    public class DecoratorDomainProviderCachingTests
    {
        private ObjectCacheAppCache? _runtimeCache;
        private DecoratorDomainProviderCaching? _testSubject;
        private Mock<IDomainProvider>? _domainProviderMock;

        [SetUp]
        public void SetUp()
        {
            _domainProviderMock = new Mock<IDomainProvider>();
            _runtimeCache = new ObjectCacheAppCache();
            _testSubject = new DecoratorDomainProviderCaching(_domainProviderMock.Object, _runtimeCache);
        }

        [TearDown]
        public void TearDown()
        {
            _runtimeCache?.Dispose();
        }

        [TestCase(TestName = "GetDomains calls decoratee only once")]
        public void GetDomains_NormalFlow_GetsDomainsFromDecorateeOnlyOnce()
        {
            // arrange
            var output = DomainCollection.Create(Enumerable.Empty<Core.Domain.Models.Domain>());
            _domainProviderMock!.Setup(obj => obj.GetDomains())
                .Returns(output);

            // act
            var result1 = _testSubject!.GetDomains();
            var result2 = _testSubject.GetDomains();

            // assert
            _domainProviderMock.Verify(obj => obj.GetDomains(), Times.Once);
            Assert.That(result1, Is.SameAs(result2));
        }
    }
}
