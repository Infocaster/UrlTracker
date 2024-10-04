using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using UrlTracker.Core.Intercepting.Conversion;
using UrlTracker.Core.Intercepting.Models;

namespace UrlTracker.Core.Tests.Intercepting.Conversion
{
    public class InterceptConverterCollectionTests
    {
        private Mock<IInterceptConverter> _interceptConverterMock;
        private InterceptConverterCollection? _testSubject;

        [SetUp]
        public void SetUp()
        {
            _interceptConverterMock = new Mock<IInterceptConverter>();
            _testSubject = new InterceptConverterCollection(() => new List<IInterceptConverter> { _interceptConverterMock.Object });
        }

        public static IEnumerable<TestCaseData> TestCases()
        {
            var cachableIntercept = new CachableInterceptBase<object>(new object());
            var intercept = new InterceptBase<object>(new object());

            yield return new TestCaseData(
                cachableIntercept,
                intercept,
                intercept).SetName("ConvertAsync returns converted value if a converter converts the input");

            yield return new TestCaseData(
                cachableIntercept,
                null,
                cachableIntercept).SetName("ConvertAsync returns input if no converter converts the input");
        }

        [TestCaseSource(nameof(TestCases))]
        public async Task ConvertAsync_NormalFlow_ReturnsIntercept(ICachableIntercept input, IIntercept output, IIntercept expected)
        {
            // arrange
            _interceptConverterMock!.Setup(obj => obj.ConvertAsync(input))
                                  .ReturnsAsync(output);

            // act
            var result = await _testSubject!.ConvertAsync(input);

            // assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
