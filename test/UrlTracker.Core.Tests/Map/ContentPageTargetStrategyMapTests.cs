using System;
using System.Collections.Generic;
using NUnit.Framework;
using Umbraco.Cms.Core.Models.PublishedContent;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Map;
using UrlTracker.Core.Models;
using UrlTracker.Resources.Testing.Mocks;
using UrlTracker.Resources.Testing.Objects;

namespace UrlTracker.Core.Tests.Map
{
    public class ContentPageTargetStrategyMapTests
    {
        private const string _testContentKey = "0b75cf71-2c9d-49d5-80a4-bb2caef917c0";
        private static readonly IPublishedContent _testContent = TestPublishedContent.Create(1234, key: Guid.Parse(_testContentKey));
        private ContentPageTargetStrategyMap _testSubject = null!;
        private UmbracoContextFactoryAbstractionMock _umbracoContextFactoryAbstractionMock;

        [SetUp]
        public void SetUp()
        {
            _umbracoContextFactoryAbstractionMock = new UmbracoContextFactoryAbstractionMock();
            _umbracoContextFactoryAbstractionMock.CrefMock.Setup(obj => obj.GetContentById(1234)).Returns(_testContent);
            _umbracoContextFactoryAbstractionMock.CrefMock.Setup(obj => obj.GetContentById(Guid.Parse(_testContentKey))).Returns(_testContent);

            _testSubject = new ContentPageTargetStrategyMap(_umbracoContextFactoryAbstractionMock.UmbracoContextFactory);
        }

        public static IEnumerable<TestCaseData> ConvertToSimpleTestCaseSource()
        {
            yield return new TestCaseData(new ContentPageTargetStrategy(TestPublishedContent.Create(1234, key: Guid.Parse("0db73f0d-da58-4959-8bb7-e7b4e4d21831")), null), 
                EntityStrategy.ContentTarget("0db73f0d-da58-4959-8bb7-e7b4e4d21831"))
                .SetName("Convert returns correct value with only id");
            yield return new TestCaseData(new ContentPageTargetStrategy(TestPublishedContent.Create(1234, key: Guid.Parse("0db73f0d-da58-4959-8bb7-e7b4e4d21831")), "en-US"), 
                EntityStrategy.ContentTarget("0db73f0d-da58-4959-8bb7-e7b4e4d21831;en-US"))
                .SetName("Convert returns correct content target value with guid as key and culture");
        }

        [TestCaseSource(nameof(ConvertToSimpleTestCaseSource))]
        public void Convert_SeveralStrategies_ReturnsCorrectResult(ContentPageTargetStrategy strategy, EntityStrategy expected)
        {
            // arrange

            // act
            var result = _testSubject.Convert(strategy);

            // assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase(TestName = "Convert throws argument exception if content does not exist")]
        public void Convert_ContentIsNull_ThrowsException()
        {
            // arrange
            ContentPageTargetStrategy input = new(null, default);

            // act
            void result() => _testSubject.Convert(input);

            // assert
            Assert.That(result, Throws.ArgumentException);
        }

        public static IEnumerable<TestCaseData> ConvertToComplexTestCaseSource()
        {
            yield return new TestCaseData(EntityStrategy.ContentTarget("1234"), new ContentPageTargetStrategy(_testContent, null))
                .SetName("Convert creates correct strategy with only content");

            yield return new TestCaseData(EntityStrategy.ContentTarget("1234;en-US"), new ContentPageTargetStrategy(_testContent, "en-US"))
                .SetName("Convert creates correct strategy with content and culture");

            yield return new TestCaseData(EntityStrategy.ContentTarget("1000"), new ContentPageTargetStrategy(null, null))
                .SetName("Convert creates correct strategy if content does not exist");

            yield return new TestCaseData(EntityStrategy.ContentTarget(_testContentKey), new ContentPageTargetStrategy(_testContent, null))
                .SetName("Convert creates correct strategy with guid as key");

            yield return new TestCaseData(EntityStrategy.ContentTarget($"{_testContentKey};en-US"), new ContentPageTargetStrategy(_testContent, "en-US"))
                .SetName("Convert creates correct strategy with guid as key and culture");
        }

        [TestCaseSource(nameof(ConvertToComplexTestCaseSource))]
        public void ConvertToComplex_SeveralStrategies_ReturnsCorrectResult(EntityStrategy strategy, ContentPageTargetStrategy expected)
        {
            // arrange

            // act
            var result = _testSubject.Convert(strategy);

            // assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
