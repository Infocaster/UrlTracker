using System;
using System.Collections.Generic;
using NUnit.Framework;
using Umbraco.Cms.Core.Configuration.Models;
using UrlTracker.Core.Models;
using UrlTracker.Resources.Testing;
using UrlTracker.Web.Processing;

namespace UrlTracker.Web.Tests.Processing
{
    public class UrlRedirectConverterTests : TestBase
    {
        private UrlRedirectConverter _testSubject = null!;

        public override void SetUp()
        {
            _testSubject = new UrlRedirectConverter(RequestHandlerSettings);
            RequestHandlerSettingsMock.Setup(obj => obj.CurrentValue).Returns(new RequestHandlerSettings { AddTrailingSlash = false });
        }

        public static IEnumerable<TestCaseData> CanHandleTestCaseSource()
        {
            yield return new TestCaseData(new Redirect { Target = new UrlTargetStrategy("http://example.com") }, true)
                .SetName("CanHandle returns true if the target strategy is a url");
            yield return new TestCaseData(new Redirect { Target = new ContentPageTargetStrategy(default, default) }, false)
                .SetName("CanHandle returns false if the target strategy is not a url");
        }

        [TestCaseSource(nameof(CanHandleTestCaseSource))]
        public void CanHandle_DifferentStrategies_ReturnsCorrectResult(Redirect redirect, bool expected)
        {
            // arrange

            // act
            var result = _testSubject.CanHandle(redirect);

            // assert
            Assert.That(result, Is.EqualTo(expected));
        }

        public static IEnumerable<TestCaseData> HandleTestCaseSource()
        {
            yield return new TestCaseData(new Redirect { Target = new UrlTargetStrategy("https://example.com/lorem") }, new Uri("https://urltracker.com/"), "https://example.com/lorem")
                .SetName("Handle returns target url if it is absolute");
            yield return new TestCaseData(new Redirect { Target = new UrlTargetStrategy("/lorem") }, new Uri("https://urltracker.com/"), "https://urltracker.com/lorem")
                .SetName("Handle returns absolute target url based on incoming request if target url is relative");
            yield return new TestCaseData(new Redirect { Target = new UrlTargetStrategy("https://example.com/$1"), Source = new RegexSourceStrategy(@"^(lorem)$") }, new Uri("https://urltracker.com/lorem"), "https://example.com/lorem")
                .SetName("Handle replaces regex capture groups on target url");
            yield return new TestCaseData(new Redirect { Target = new UrlTargetStrategy("https://example.com/"), RetainQuery = true }, new Uri("https://urltracker.com/?lorem=ipsum"), "https://example.com?lorem=ipsum")
                .SetName("Handle includes query string if that is enabled");
        }

        [TestCaseSource(nameof(HandleTestCaseSource))]
        public void Handle_DifferentStrategies_ReturnsCorrectResult(Redirect redirect, Uri incomingUrl, string? expected)
        {
            // arrange
            HttpContextMock.SetupUrl(incomingUrl);

            // act
            var result = _testSubject.Handle(redirect, HttpContextMock.Context);

            // assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
