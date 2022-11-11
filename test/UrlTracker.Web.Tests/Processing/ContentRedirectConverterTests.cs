using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using UrlTracker.Core.Models;
using UrlTracker.Resources.Testing;
using UrlTracker.Resources.Testing.Objects;
using UrlTracker.Web.Processing;

namespace UrlTracker.Web.Tests.Processing
{
    public class ContentRedirectConverterTests : TestBase
    {
        private ContentRedirectConverter _testSubject = null!;

        public override void SetUp()
        {
            _testSubject = new ContentRedirectConverter(UmbracoContextFactoryAbstractionMock.UmbracoContextFactory, RequestHandlerSettings);
            RequestHandlerSettingsMock.Setup(obj => obj.CurrentValue).Returns(new RequestHandlerSettings { AddTrailingSlash = false });
        }

        public static IEnumerable<TestCaseData> CanHandleTestCaseSource()
        {
            yield return new TestCaseData(new Redirect { Target = new ContentPageTargetStrategy(default, default) }, true)
                .SetName("CanHandle returns true if the target strategy is content");
            yield return new TestCaseData(new Redirect { Target = new UrlTargetStrategy("http://example.com") }, false)
                .SetName("CanHandle returns false if the target strategy is not content");
        }

        [TestCaseSource(nameof(CanHandleTestCaseSource))]
        public void CanHandle_SeveralStrategies_ReturnsCorrectResult(Redirect redirect, bool expected)
        {
            // arrange

            // act
            var result = _testSubject.CanHandle(redirect);

            // assert
            Assert.That(result, Is.EqualTo(expected));
        }

        public static IEnumerable<TestCaseData> HandleTestCaseSource()
        {
            Uri standardIncomingUrl = new Uri("https://urltracker.com/");
            yield return new TestCaseData(new Redirect { Target = new ContentPageTargetStrategy(null, null) }, null, standardIncomingUrl, null)
                .SetName("Handle returns null if content is null");
            yield return new TestCaseData(new Redirect { Target = new ContentPageTargetStrategy(TestPublishedContent.Create(1234), "en-US") }, "https://example.com/lorem", standardIncomingUrl, "https://example.com/lorem")
                .SetName("Handle returns the content url for the appropriate culture");
            yield return new TestCaseData(new Redirect { Target = new ContentPageTargetStrategy(TestPublishedContent.Create(1234), "en-US"), RetainQuery = true }, "https://example.com/lorem", new Uri("https://urltracker.com/?lorem=ipsum"), "https://example.com/lorem?lorem=ipsum")
                .SetName("Handle includes query string if that is enabled");
        }

        [TestCaseSource(nameof(HandleTestCaseSource))]
        public void Handle_SeveralCases_ReturnsCorrectResult(Redirect redirect, string? contentUrl, Uri incomingUrl, string? expected)
        {
            // arrange
            HttpContextMock.SetupUrl(incomingUrl);
            if (contentUrl is not null)
            {
                UmbracoContextFactoryAbstractionMock.CrefMock
                    .Setup(obj => obj.GetUrl(It.IsAny<IPublishedContent>(), UrlMode.Absolute, It.IsAny<string?>()))
                    .Returns(contentUrl);
            }

            // act
            var result = _testSubject.Handle(redirect, HttpContextMock.Context);

            // assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
