using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Mapping;
using UrlTracker.Core.Abstractions;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Core.Models;
using UrlTracker.Resources.Testing;
using UrlTracker.Resources.Testing.Logging;
using UrlTracker.Resources.Testing.Objects;
using UrlTracker.Web.Processing;

namespace UrlTracker.Web.Tests.Processing
{
    public class RedirectResponseInterceptHandlerTests : TestBase
    {
        private TestMapDefinition<Redirect, Url>? _testMap;
        private TestRedirectResponseInterceptHandler? _testSubject;

        protected override ICollection<IMapDefinition> CreateMappers()
        {
            return new IMapDefinition[]
            {
                _testMap = CreateTestMap<Redirect, Url>()
            };
        }

        public override void SetUp()
        {
            RequestHandlerSettingsMock.Setup(obj => obj.CurrentValue).Returns(new RequestHandlerSettings { AddTrailingSlash = false });
            _testSubject = new TestRedirectResponseInterceptHandler(new VoidLogger<TestRedirectResponseInterceptHandler>(), ResponseAbstraction, UmbracoContextFactoryAbstractionMock!.UmbracoContextFactory);
        }

        public static IEnumerable<TestCaseData> TestCases()
        {
            yield return new RedirectResponseHandlerTestCase
            {
                ExpectedStatusCode = 301,
                ExpectedUrl = "http://example.com/lorem",
                InitialStatusCode = 404,
                InitialUrl = "http://example.com",
                Redirect = new Redirect
                {
                    Force = false,
                    Permanent = true
                }
            }.ToTestCase("HandleAsync redirects if status code is 404");

            yield return new RedirectResponseHandlerTestCase
            {
                ExpectedStatusCode = 200,
                ExpectedUrl = null,
                InitialStatusCode = 200,
                InitialUrl = "http://example.com",
                Redirect = new Redirect
                {
                    Force = false,
                    Permanent = true
                }
            }.ToTestCase("HandleAsync does not redirect if status code is 200 and redirect is not forced");

            yield return new RedirectResponseHandlerTestCase
            {
                ExpectedStatusCode = 301,
                ExpectedUrl = "http://example.com/lorem",
                InitialStatusCode = 200,
                InitialUrl = "http://example.com",
                Redirect = new Redirect
                {
                    Force = true,
                    Permanent = true
                }
            }.ToTestCase("HandleAsync redirects if status code is 200 and redirect is forced");

            yield return new RedirectResponseHandlerTestCase
            {
                ExpectedStatusCode = 301,
                ExpectedUrl = "http://example.com/lorem",
                InitialStatusCode = 404,
                InitialUrl = "http://example.com?ipsum=dolor",
                Redirect = new Redirect
                {
                    Force = false,
                    Permanent = true,
                    RetainQuery = false
                }
            }.ToTestCase("HandleAsync does not pass through query string if this is disabled in the redirect");

            yield return new RedirectResponseHandlerTestCase
            {
                ExpectedStatusCode = 301,
                ExpectedUrl = "http://example.com/lorem?ipsum=dolor",
                InitialStatusCode = 404,
                InitialUrl = "http://example.com?ipsum=dolor",
                Redirect = new Redirect
                {
                    Force = false,
                    Permanent = true,
                    RetainQuery = true
                }
            }.ToTestCase("HandleAsync passes through query string if this is enabled in the redirect");

            yield return new RedirectResponseHandlerTestCase
            {
                ExpectedStatusCode = 410,
                ExpectedUrl = null,
                InitialStatusCode = 404,
                InitialUrl = "http://example.com/lorem",
                Redirect = new Redirect
                {
                    Permanent = false
                }
            }.ToTestCase("HandleAsync rewrites response to 410 if the published content target no longer exists");
        }

        [TestCaseSource(nameof(TestCases))]
        public async Task HandleAsync_NormalFlow_ProcessesIntercept(Redirect redirect, int initialStatusCode, int expectedStatusCode, string initialUrl, string expectedUrl)
        {
            // arrange
            HttpContextMock!.ResponseMock.SetupProperty(obj => obj.StatusCode, initialStatusCode);
            HttpContextMock.SetupUrl(new Uri(initialUrl));
            UmbracoContextFactoryAbstractionMock!.CrefMock.Setup(obj => obj.GetResponseCode()).Returns(initialStatusCode);
            bool nextInvoked = false;
            Task next(HttpContext context) => Task.FromResult(nextInvoked = true);
            if (expectedUrl is not null)
            {
                _testSubject.returnValue = expectedUrl;
                ResponseAbstractionMock!.Setup(obj => obj.SetRedirectLocation(HttpContextMock.Response, expectedUrl)).Verifiable();
            }
            var input = new InterceptBase<Redirect>(redirect);

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

        private class TestRedirectResponseInterceptHandler
            : RedirectResponseInterceptHandler<TestTarget>
        {
            public TestRedirectResponseInterceptHandler(ILogger logger,
                                                        Abstraction.IResponseAbstraction responseAbstraction,
                                                        IUmbracoContextFactoryAbstraction umbracoContextFactory)
                : base(logger, responseAbstraction, umbracoContextFactory)
            {
            }

            protected override string? GetUrl(HttpContext context, Redirect intercept, TestTarget target)
            {
                return returnValue;
            }

            public string? returnValue { get; set; }
        }

        private class TestTarget : ITargetStrategy
        { }
    }
}
