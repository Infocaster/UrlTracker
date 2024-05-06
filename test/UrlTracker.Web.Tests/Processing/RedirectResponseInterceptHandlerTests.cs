using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Mapping;
using UrlTracker.Core.Abstractions;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Core.Models;
using UrlTracker.Resources.Testing.Logging;
using UrlTracker.Resources.Testing.Mocks;
using UrlTracker.Resources.Testing.Objects;
using UrlTracker.Web.Abstraction;
using UrlTracker.Web.Processing.Handling;

namespace UrlTracker.Web.Tests.Processing
{
    public class RedirectResponseInterceptHandlerTests
    {
        private TestMapDefinition<Redirect, Url>? _testMap;
        private Mock<IResponseAbstraction> _responseAbstractionMock;
        private UmbracoContextFactoryAbstractionMock _umbracoContextFactoryAbstractionMock;
        private TestRedirectResponseInterceptHandler? _testSubject;
        private Mock<IOptionsMonitor<RequestHandlerSettings>> _requestHandlerSettingsMock;

        private ICollection<IMapDefinition> CreateMappers()
        {
            return new IMapDefinition[]
            {
                _testMap = TestMapDefinition.CreateTestMap<Redirect, Url>()
            };
        }

        [SetUp]
        public void SetUp()
        {
            _requestHandlerSettingsMock = new Mock<IOptionsMonitor<RequestHandlerSettings>>();
            _requestHandlerSettingsMock.Setup(obj => obj.CurrentValue).Returns(new RequestHandlerSettings { AddTrailingSlash = false });
            _responseAbstractionMock = new Mock<IResponseAbstraction>();
            _umbracoContextFactoryAbstractionMock = new UmbracoContextFactoryAbstractionMock();
            _testSubject = new TestRedirectResponseInterceptHandler(new VoidLogger<TestRedirectResponseInterceptHandler>(), _responseAbstractionMock.Object, _umbracoContextFactoryAbstractionMock.UmbracoContextFactory);
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
            var httpContextMock = new HttpContextMock();
            httpContextMock!.ResponseMock.SetupProperty(obj => obj.StatusCode, initialStatusCode);
            httpContextMock.SetupUrl(new Uri(initialUrl));
            _umbracoContextFactoryAbstractionMock!.CrefMock.Setup(obj => obj.GetResponseCode()).Returns(initialStatusCode);
            bool nextInvoked = false;
            Task next(HttpContext context) => Task.FromResult(nextInvoked = true);
            if (expectedUrl is not null)
            {
                _testSubject!.returnValue = expectedUrl;
                _responseAbstractionMock!.Setup(obj => obj.SetRedirectLocation(httpContextMock.Response, expectedUrl)).Verifiable();
            }
            var input = new InterceptBase<Redirect>(redirect);

            // act
            await _testSubject!.HandleAsync(next, httpContextMock.Context, input);

            // assert
            httpContextMock.ResponseMock.Verify();
            Assert.Multiple(() =>
            {
                if (initialStatusCode == expectedStatusCode)
                {
                    Assert.That(nextInvoked, Is.True);
                }
                Assert.That(httpContextMock.Response.StatusCode, Is.EqualTo(expectedStatusCode));
                _responseAbstractionMock!.Verify();
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
