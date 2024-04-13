using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Core.Intercepting.Preprocessing;

namespace UrlTracker.Core.Tests.Intercepting.Preprocessing
{
    public class InterceptPreprocessorCollectionTests
    {
        private Mock<IInterceptPreprocessor> _interceptPreprocessorMock;
        private Mock<IDefaultInterceptContextFactory> _defaultInterceptContextFactoryMock;
        private InterceptPreprocessorCollection? _testSubject;

        [SetUp]
        public void SetUp()
        {
            _interceptPreprocessorMock = new Mock<IInterceptPreprocessor>();
            _defaultInterceptContextFactoryMock = new Mock<IDefaultInterceptContextFactory>();
            _testSubject = new InterceptPreprocessorCollection(() => new List<IInterceptPreprocessor> { _interceptPreprocessorMock.Object }, _defaultInterceptContextFactoryMock.Object);
        }

        public static IEnumerable<TestCaseData> TestCases()
        {
            var inputContext = new DefaultInterceptContext();
            var factoryContext = new DefaultInterceptContext();

            yield return new TestCaseData(
                inputContext,
                null,
                inputContext).SetName("PreprocessUrlAsync returns populated input context if one is provided");

            yield return new TestCaseData(
                null,
                factoryContext,
                factoryContext).SetName("PreprocessUrlAsync creates new populated context if none is provided");
        }

        [TestCaseSource(nameof(TestCases))]
        public async Task PreprocessUrlAsync_NormalFlow_ReturnsPopulatedContext(IInterceptContext inputContext, IInterceptContext factoryContext, IInterceptContext expected)
        {
            // arrange
            _interceptPreprocessorMock!.Setup(obj => obj.PreprocessUrlAsync(It.IsAny<Url>(), It.IsAny<IInterceptContext>()))
                                     .ReturnsAsync((Url url, IInterceptContext context) => context)
                                     .Verifiable();
            _defaultInterceptContextFactoryMock!.Setup(obj => obj.Create())
                                              .Returns(factoryContext);

            // act
            var result = await _testSubject!.PreprocessUrlAsync(Url.Parse("http://example.com"), inputContext);

            // assert
            Assert.That(result, Is.EqualTo(expected));
            _interceptPreprocessorMock.Verify();
        }
    }
}
