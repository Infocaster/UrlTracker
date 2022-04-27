using System.Collections.Generic;
using NUnit.Framework;
using Umbraco.Core.Mapping;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Models;
using UrlTracker.Resources.Testing;
using UrlTracker.Resources.Testing.Mocks;
using UrlTracker.Resources.Testing.Objects;

namespace UrlTracker.Web.Tests
{
    public class UmbracoMapperExtensionsTests : TestBase
    {
        private MapperContext _mapperContext;
        private TestMapDefinition<ShallowRedirect, Url> _testMap;

        protected override ICollection<IMapDefinition> CreateMappers()
        {
            return new IMapDefinition[]
            {
                _testMap = CreateTestMap<ShallowRedirect, Url>()
            };
        }

        public override void SetUp()
        {
            _mapperContext = new MapperContext(Mapper);
        }

        [TestCase(TestName = "Mapper.MapToUrl throws NullReferenceException when no HttpContext was provided")]
        public void MapperMapToUrl_NoHttpContext_ThrowsException()
        {
            // arrange

            // act
            Url result() => Mapper.MapToUrl(new ShallowRedirect(), null);

            // assert
            Assert.That(result, Throws.ArgumentNullException);
        }

        [TestCase(TestName = "Mapper.MapToUrl returns url for given redirect and context")]
        public void MapperMapToUrl_NormalFlow_ReturnsResult()
        {
            // arrange
            _testMap.To = Url.Parse("http://example.com");

            // act
            var result = Mapper.MapToUrl(new ShallowRedirect(), HttpContextMock.Context);

            // assert
            Assert.That(result, Is.EqualTo(_testMap.To));
        }

        [TestCase(TestName = "Context.MapToUrl throws NullReferenceException when no HttpContext was provided")]
        public void ContextMapToUrl_NoHttpContext_ThrowsException()
        {
            // arrange

            // act
            Url result() => _mapperContext.MapToUrl(new ShallowRedirect(), null);

            // assert
            Assert.That(result, Throws.ArgumentNullException);
        }

        [TestCase(TestName = "Context.MapToUrl returns url for given redirect and context")]
        public void ContextMapToUrl_NormalFlow_ReturnsResult()
        {
            // arrange
            _testMap.To = Url.Parse("http://example.com");

            // act
            var result = _mapperContext.MapToUrl(new ShallowRedirect(), HttpContextMock.Context);

            // assert
            Assert.That(result, Is.EqualTo(_testMap.To));
        }
    }
}
