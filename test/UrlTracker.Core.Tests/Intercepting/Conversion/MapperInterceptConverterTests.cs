using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Umbraco.Core.Mapping;
using UrlTracker.Core.Intercepting.Conversion;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Resources.Testing;
using UrlTracker.Resources.Testing.Objects;

namespace UrlTracker.Core.Tests.Intercepting.Conversion
{
    public class MapperInterceptConverterTests : TestBase
    {
        private TestMapDefinition<TestResponseIntercept1, TestResponseIntercept2> _testMapDefinition;
        private MapperInterceptConverter<TestResponseIntercept1, TestResponseIntercept2> _testSubject;

        protected override ICollection<IMapDefinition> CreateMappers()
        {
            return new[]
            {
                _testMapDefinition = CreateTestMap<TestResponseIntercept1, TestResponseIntercept2>()
            };
        }

        public override void SetUp()
        {
            _testSubject = new MapperInterceptConverter<TestResponseIntercept1, TestResponseIntercept2>(Mapper);
        }

        [TestCase(TestName = "ConvertAsync returns null if it is not a converter for the given argument")]
        public async Task ConvertAsync_NotMatchingConverter_ReturnsNull()
        {
            // arrange
            var input = new CachableInterceptBase<object>(new object());

            // act
            var result = await _testSubject.ConvertAsync(input);

            // assert
            Assert.That(result, Is.Null);
        }

        [TestCase(TestName = "ConvertAsync returns a value if it is a converter for the given argument")]
        public async Task ConvertAsync_MatchingConverter_ReturnsMappedResult()
        {
            // arrange
            var input = new TestResponseIntercept1();
            var output = new TestResponseIntercept2();
            _testMapDefinition.To = output;

            // act
            var result = await _testSubject.ConvertAsync(new CachableInterceptBase<TestResponseIntercept1>(input));

            // assert
            Assert.That(result.Info, Is.SameAs(output));
        }

        [TestCase(TestName = "ConvertAsync returns null if the argument is null")]
        public async Task ConvertAsync_ArgumentNull_ReturnsNull()
        {
            // arrange

            // act
            var result = await _testSubject.ConvertAsync(null);

            // assert
            Assert.That(result, Is.Null);
        }
    }
}
