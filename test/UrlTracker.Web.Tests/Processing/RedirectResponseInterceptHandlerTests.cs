using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Mapping;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Core.Models;
using UrlTracker.Resources.Testing;
using UrlTracker.Resources.Testing.Logging;
using UrlTracker.Resources.Testing.Mocks;
using UrlTracker.Resources.Testing.Objects;
using UrlTracker.Web.Processing;

namespace UrlTracker.Web.Tests.Processing
{
    public class RedirectResponseInterceptHandlerTests : TestBase
    {
        private TestMapDefinition<ShallowRedirect, Url>? _testMap;
        private RedirectResponseInterceptHandler? _testSubject;

        protected override ICollection<IMapDefinition> CreateMappers()
        {
            return new IMapDefinition[]
            {
                _testMap = CreateTestMap<ShallowRedirect, Url>()
            };
        }

        public override void SetUp()
        {
            RequestHandlerSettingsMock.Setup(obj => obj.CurrentValue).Returns(new RequestHandlerSettings { AddTrailingSlash = false });
            _testSubject = new RedirectResponseInterceptHandler(new ConsoleLogger<RedirectResponseInterceptHandler>(), Mapper!, ResponseAbstraction, UmbracoContextFactoryAbstractionMock!.UmbracoContextFactory, RequestHandlerSettings);
            _testMap!.To = null; // <- always reset the url on the test map to prevent urls from leaking between tests
        }

        public static IEnumerable<TestCaseData> TestCases()
        {
            yield return new RedirectResponseHandlerTestCase
            {
                ExpectedStatusCode = 301,
                ExpectedUrl = "http://example.com/lorem",
                InitialStatusCode = 404,
                InitialUrl = "http://example.com",
                Redirect = new ShallowRedirect
                {
                    Force = false,
                    TargetStatusCode = HttpStatusCode.MovedPermanently
                }
            }.ToTestCase("HandleAsync redirects if status code is 404");

            yield return new RedirectResponseHandlerTestCase
            {
                ExpectedStatusCode = 200,
                ExpectedUrl = null,
                InitialStatusCode = 200,
                InitialUrl = "http://example.com",
                Redirect = new ShallowRedirect
                {
                    Force = false,
                    TargetStatusCode = HttpStatusCode.MovedPermanently
                }
            }.ToTestCase("HandleAsync does not redirect if status code is 200 and redirect is not forced");

            yield return new RedirectResponseHandlerTestCase
            {
                ExpectedStatusCode = 301,
                ExpectedUrl = "http://example.com/lorem",
                InitialStatusCode = 200,
                InitialUrl = "http://example.com",
                Redirect = new ShallowRedirect
                {
                    Force = true,
                    TargetStatusCode = HttpStatusCode.MovedPermanently
                }
            }.ToTestCase("HandleAsync redirects if status code is 200 and redirect is forced");

            yield return new RedirectResponseHandlerTestCase
            {
                ExpectedStatusCode = 301,
                ExpectedUrl = "http://example.com/lorem",
                InitialStatusCode = 404,
                InitialUrl = "http://example.com?ipsum=dolor",
                Redirect = new ShallowRedirect
                {
                    Force = false,
                    TargetStatusCode = HttpStatusCode.MovedPermanently,
                    PassThroughQueryString = false
                }
            }.ToTestCase("HandleAsync does not pass through query string if this is disabled in the redirect");

            yield return new RedirectResponseHandlerTestCase
            {
                ExpectedStatusCode = 301,
                ExpectedUrl = "http://example.com/lorem?ipsum=dolor",
                InitialStatusCode = 404,
                InitialUrl = "http://example.com?ipsum=dolor",
                Redirect = new ShallowRedirect
                {
                    Force = false,
                    TargetStatusCode = HttpStatusCode.MovedPermanently,
                    PassThroughQueryString = true
                }
            }.ToTestCase("HandleAsync passes through query string if this is enabled in the redirect");

            yield return new RedirectResponseHandlerTestCase
            {
                ExpectedStatusCode = 410,
                ExpectedUrl = null,
                InitialStatusCode = 404,
                InitialUrl = "http://example.com/lorem",
                Redirect = new ShallowRedirect
                {
                    TargetStatusCode = HttpStatusCode.Redirect
                }
            }.ToTestCase("HandleAsync rewrites response to 410 if the published content target no longer exists");

            yield return new RedirectResponseHandlerTestCase
            {
                ExpectedStatusCode = 302,
                ExpectedUrl = "http://example.com/lorem",
                InitialStatusCode = 404,
                InitialUrl = "http://example.com/123456/lorem",
                Redirect = new ShallowRedirect
                {
                    TargetStatusCode = HttpStatusCode.Redirect,
                    SourceRegex = @"^\d{6}/(\w+)",
                    TargetUrl = "http://example.com/$1"
                }
            }.ToTestCase("HandleAsync replaces regex capture groups if the source is a regex");

            yield return new RedirectResponseHandlerTestCase
            {
                ExpectedStatusCode = 302,
                ExpectedUrl = "http://example.com/$1",
                InitialStatusCode = 404,
                InitialUrl = "http://example.com/123456/lorem",
                Redirect = new ShallowRedirect
                {
                    TargetStatusCode = HttpStatusCode.Redirect,
                    SourceRegex = @"^\d{6}/(\w+)",
                    SourceUrl = "http://example.com/123456/lorem",
                    TargetUrl = "http://example.com/$1"
                }
            }.ToTestCase("HandleAsync does not replace regex capture groups if the source is not a regex");
        }

        [TestCaseSource(nameof(TestCases))]
        public async Task HandleAsync_NormalFlow_ProcessesIntercept(ShallowRedirect redirect, int initialStatusCode, int expectedStatusCode, string initialUrl, string expectedUrl)
        {
            // arrange
            HttpContextMock!.ResponseMock.SetupProperty(obj => obj.StatusCode, initialStatusCode);
            HttpContextMock.SetupUrl(new Uri(initialUrl));
            UmbracoContextFactoryAbstractionMock!.CrefMock.Setup(obj => obj.GetResponseCode()).Returns(initialStatusCode);
            bool nextInvoked = false;
            Task next(HttpContext context) => Task.FromResult(nextInvoked = true);
            if (expectedUrl is not null)
            {
                _testMap!.To = Url.Parse(expectedUrl);
                ResponseAbstractionMock!.Setup(obj => obj.SetRedirectLocation(HttpContextMock.Response, expectedUrl)).Verifiable();
            }
            var input = new InterceptBase<ShallowRedirect>(redirect);

            // act
            await _testSubject!.HandleAsync(next, HttpContextMock.Context, input);

            // assert
            HttpContextMock.ResponseMock.Verify();
            Assert.Multiple(() =>
            {
                if (initialStatusCode == expectedStatusCode)
                {
                    Assert.That(nextInvoked, Is.True);
                }
                Assert.That(HttpContextMock.Response.StatusCode, Is.EqualTo(expectedStatusCode));
                ResponseAbstractionMock!.Verify();
            });
        }
    }
}
