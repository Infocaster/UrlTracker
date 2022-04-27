using Moq;
using NUnit.Framework;
using UrlTracker.Core.Configuration;

namespace UrlTracker.Core.Tests.Configuration
{
    public class LazyConfigurationProviderTests
    {
        private Mock<IConfiguration<object>> _configurationMock;
        private LazyConfigurationProvider<object> _testSubject;

        [SetUp]
        public void SetUp()
        {
            _configurationMock = new Mock<IConfiguration<object>>();
            _testSubject = new LazyConfigurationProvider<object>(_configurationMock.Object);
        }

        [TestCase(TestName = "Value only calls decoratee once")]
        public void Value_NormalFlow_OnlyCallsDecorateeOnce()
        {
            // arrange
            _configurationMock.SetupGet(obj => obj.Value).Returns(() => new object()).Verifiable();

            // act
            var result1 = _testSubject.Value;
            var result2 = _testSubject.Value;

            // assert
            _configurationMock.Verify(obj => obj.Value, Times.Once);
            Assert.That(result1, Is.SameAs(result2));
        }
    }
}
