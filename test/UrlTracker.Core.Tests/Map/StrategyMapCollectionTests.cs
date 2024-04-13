using Moq;
using NUnit.Framework;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Map;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Tests.Map
{
    public class StrategyMapCollectionTests
    {
        private Mock<IStrategyMap<ISourceStrategy>> _strategyMapMock = null!;
        private StrategyMapCollection _testSubject = null!;

        [SetUp]
        public void SetUp()
        {
            _strategyMapMock = new Mock<IStrategyMap<ISourceStrategy>>();
            _testSubject = new StrategyMapCollection(() => new[] { _strategyMapMock.Object });
        }

        [TestCase(TestName = "Map calls mapper if a suitable instance exists")]
        public void MapToSimple_MapperExists_CallsMapper()
        {
            // arrange
            _strategyMapMock.Setup(obj => obj.CanHandle(It.IsAny<object>())).Returns(true);
            _strategyMapMock.Setup(obj => obj.Convert(It.IsAny<object>())).Verifiable();

            // act
            _testSubject.Map(Mock.Of<object>());

            // assert
            _strategyMapMock.Verify();
        }

        [TestCase(TestName = "Map throws an exception if no suitable instance exists")]
        public void MapToSimple_MapperDoesNotExist_ThrowsException()
        {
            // arrange
            _strategyMapMock.Setup(obj => obj.CanHandle(It.IsAny<object>())).Returns(false);

            // act
            void result() => _testSubject.Map(Mock.Of<object>());

            // assert
            Assert.That(result, Throws.ArgumentException);
        }

        [TestCase(TestName = "Map calls mapper if a suitable instance exists")]
        public void MapToComplex_MapperExists_CallsMapper()
        {
            // arrange
            _strategyMapMock.Setup(obj => obj.CanHandle(It.IsAny<EntityStrategy>())).Returns(true);
            _strategyMapMock.Setup(obj => obj.Convert(It.IsAny<EntityStrategy>())).Verifiable();

            // act
            _testSubject.Map<ISourceStrategy>(EntityStrategy.UrlSource("https://example.com"));

            // assert
            _strategyMapMock.Verify();
        }

        [TestCase(TestName = "Map throws an exception if no suitable instance exists")]
        public void MapToComplex_MapperDoesNotExist_ThrowsException()
        {
            // arrange
            _strategyMapMock.Setup(obj => obj.CanHandle(It.IsAny<EntityStrategy>())).Returns(false);

            // act
            void result() => _testSubject.Map<ISourceStrategy>(EntityStrategy.UrlSource("https://example.com"));

            // assert
            Assert.That(result, Throws.ArgumentException);
        }
    }
}
