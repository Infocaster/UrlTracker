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
        private static readonly IPublishedContent _testContent = TestPublishedContent.Create(1234, Guid.Parse("5a911069-f9ad-4616-b741-96ff7384d7ba"));
        private ContentPageTargetStrategyMap _testSubject = null!;
        private UmbracoContextFactoryAbstractionMock _umbracoContextFactoryAbstractionMock;

        [SetUp]
        public void SetUp()
        {
            var expectedGuid = Guid.Parse("5a911069-f9ad-4616-b741-96ff7384d7ba");
            _umbracoContextFactoryAbstractionMock = new UmbracoContextFactoryAbstractionMock();
            _umbracoContextFactoryAbstractionMock.CrefMock.Setup(obj => obj.GetContentById(expectedGuid)).Returns(_testContent);

            _testSubject = new ContentPageTargetStrategyMap(_umbracoContextFactoryAbstractionMock.UmbracoContextFactory);
        }

        public static IEnumerable<TestCaseData> ConvertToSimpleTestCaseSource()
        {
            yield return new TestCaseData(new ContentPageTargetStrategy(TestPublishedContent.Create(1234, Guid.Parse("5a911069-f9ad-4616-b741-96ff7384d7ba")), null), EntityStrategy.ContentTarget("5A911069-F9AD-4616-B741-96FF7384D7BA"))
                .SetName("Convert returns correct value with only id");
            yield return new TestCaseData(new ContentPageTargetStrategy(TestPublishedContent.Create(1234, Guid.Parse("5a911069-f9ad-4616-b741-96ff7384d7ba")), "en-US"), EntityStrategy.ContentTarget("5A911069-F9AD-4616-B741-96FF7384D7BA;en-US"))
                .SetName("Convert returns correct value with id and culture");
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
            yield return new TestCaseData(EntityStrategy.ContentTarget("5a911069-f9ad-4616-b741-96ff7384d7ba"), new ContentPageTargetStrategy(_testContent, null))
                .SetName("Convert creates correct strategy with only content");

            yield return new TestCaseData(EntityStrategy.ContentTarget("5a911069-f9ad-4616-b741-96ff7384d7ba;en-US"), new ContentPageTargetStrategy(_testContent, "en-US"))
                .SetName("Convert creates correct strategy with content and culture");

            yield return new TestCaseData(EntityStrategy.ContentTarget("c3a3babb-42b0-4125-b2e6-c4287a7a58b1"), new ContentPageTargetStrategy(null, null))
                .SetName("Convert creates correct strategy if content does not exist");
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
