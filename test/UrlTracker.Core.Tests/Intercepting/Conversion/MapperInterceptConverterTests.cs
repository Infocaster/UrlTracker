using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Scoping;
using UrlTracker.Core.Intercepting.Conversion;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Resources.Testing.Logging;
using UrlTracker.Resources.Testing.Objects;

namespace UrlTracker.Core.Tests.Intercepting.Conversion
{
    public class MapperInterceptConverterTests
    {
        private TestMapDefinition<TestResponseIntercept1, TestResponseIntercept2>? _testMapDefinition;
        private UmbracoMapper _mapper;
        private MapperInterceptConverter<TestResponseIntercept1, TestResponseIntercept2>? _testSubject;

        private ICollection<IMapDefinition> CreateMappers()
        {
            return new[]
            {
                _testMapDefinition = TestMapDefinition.CreateTestMap<TestResponseIntercept1, TestResponseIntercept2>()
            };
        }

        [SetUp]
        public void SetUp()
        {
            _mapper = new UmbracoMapper(new MapDefinitionCollection(CreateMappers), Mock.Of<ICoreScopeProvider>(), new VoidLogger<UmbracoMapper>());
            _testSubject = new MapperInterceptConverter<TestResponseIntercept1, TestResponseIntercept2>(_mapper);
        }

        [TestCase(TestName = "ConvertAsync returns null if it is not a converter for the given argument")]
        public async Task ConvertAsync_NotMatchingConverter_ReturnsNull()
        {
            // arrange
            var input = new CachableInterceptBase<object>(new object());

            // act
            var result = await _testSubject!.ConvertAsync(input);

            // assert
            Assert.That(result, Is.Null);
        }

        [TestCase(TestName = "ConvertAsync returns a value if it is a converter for the given argument")]
        public async Task ConvertAsync_MatchingConverter_ReturnsMappedResult()
        {
            // arrange
            var input = new TestResponseIntercept1();
            var output = new TestResponseIntercept2();
            _testMapDefinition!.To = output;

            // act
            var result = await _testSubject!.ConvertAsync(new CachableInterceptBase<TestResponseIntercept1>(input));

            // assert
            Assert.That(result!.Info, Is.SameAs(output));
        }
    }
}
