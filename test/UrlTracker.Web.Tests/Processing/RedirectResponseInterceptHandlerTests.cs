using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Moq;
using NUnit.Framework;
using Umbraco.Core.Mapping;
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
        private TestMapDefinition<Redirect, Url> _testMap;
        private RedirectResponseInterceptHandler _testSubject;

        protected override ICollection<IMapDefinition> CreateMappers()
        {
            return new IMapDefinition[]
            {
                _testMap = CreateTestMap<Redirect, Url>()
            };
        }

        public override void SetUp()
        {
            RequestHandlerSectionMock.Setup(obj => obj.AddTrailingSlash).Returns(false);
            _testSubject = new RedirectResponseInterceptHandler(new VoidLogger(), Mapper, CompleteRequestAbstraction, UmbracoSettingsSection);
            _testMap.To = null; // <- always reset the url on the test map to prevent urls from leaking between tests
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
                    TargetStatusCode = HttpStatusCode.MovedPermanently
                }
            }.ToTestCase("HandleAsync 404 not forced redirects");

            yield return new RedirectResponseHandlerTestCase
            {
                ExpectedStatusCode = 200,
                ExpectedUrl = null,
                InitialStatusCode = 200,
                InitialUrl = "http://example.com",
                Redirect = new Redirect
                {
                    Force = false,
                    TargetStatusCode = HttpStatusCode.MovedPermanently
                }
            }.ToTestCase("HandleAsync 200 not forced does not redirect");

            yield return new RedirectResponseHandlerTestCase
            {
                ExpectedStatusCode = 301,
                ExpectedUrl = "http://example.com/lorem",
                InitialStatusCode = 200,
                InitialUrl = "http://example.com",
                Redirect = new Redirect
                {
                    Force = true,
                    TargetStatusCode = HttpStatusCode.MovedPermanently
                }
            }.ToTestCase("HandleAsync 200 forced redirects");

            yield return new RedirectResponseHandlerTestCase
            {
                ExpectedStatusCode = 301,
                ExpectedUrl = "http://example.com/lorem",
                InitialStatusCode = 404,
                InitialUrl = "http://example.com?ipsum=dolor",
                Redirect = new Redirect
                {
                    Force = false,
                    TargetStatusCode = HttpStatusCode.MovedPermanently,
                    RetainQuery = false
                }
            }.ToTestCase("HandleAsync query string not passed through");

            yield return new RedirectResponseHandlerTestCase
            {
                ExpectedStatusCode = 301,
                ExpectedUrl = "http://example.com/lorem?ipsum=dolor",
                InitialStatusCode = 404,
                InitialUrl = "http://example.com?ipsum=dolor",
                Redirect = new Redirect
                {
                    Force = false,
                    TargetStatusCode = HttpStatusCode.MovedPermanently,
                    RetainQuery = true
                }
            }.ToTestCase("HandleAsync query string passed through");

            yield return new RedirectResponseHandlerTestCase
            {
                ExpectedStatusCode = 410,
                ExpectedUrl = null,
                InitialStatusCode = 404,
                InitialUrl = "http://example.com/lorem",
                Redirect = new Redirect
                {
                    TargetStatusCode = HttpStatusCode.Redirect
                }
            }.ToTestCase("HandleAsync redirect target gone");
        }

        [TestCaseSource(nameof(TestCases))]
        public async Task HandleAsync_NormalFlow_ProcessesIntercept(Redirect redirect, int initialStatusCode, int expectedStatusCode, string initialUrl, string expectedUrl)
        {
            // arrange
            HttpContextMock.ResponseMock.SetupProperty(obj => obj.StatusCode, initialStatusCode);
            HttpContextMock.RequestMock.SetupGet(obj => obj.Url).Returns(new Uri(initialUrl));

            if (initialStatusCode != expectedStatusCode)
            {
                HttpContextMock.ResponseMock.Setup(obj => obj.Clear()).Verifiable();
                CompleteRequestAbstractionMock.Setup(obj => obj.CompleteRequest(It.Is<HttpContextBase>(o => ReferenceEquals(o, HttpContextMock.Context)))).Verifiable();
            }
            if (expectedUrl != null)
            {
                _testMap.To = Url.Parse(expectedUrl);
                HttpContextMock.ResponseMock.SetupProperty(obj => obj.RedirectLocation);
            }
            var input = new InterceptBase<Redirect>(redirect);

            // act
            await _testSubject.HandleAsync(HttpContextMock.Context, input);

            // assert
            HttpContextMock.ResponseMock.Verify();
            CompleteRequestAbstractionMock.Verify();
            Assert.Multiple(() =>
            {
                Assert.That(HttpContextMock.Response.StatusCode, Is.EqualTo(expectedStatusCode));
                Assert.That(HttpContextMock.Response.RedirectLocation, Is.EqualTo(expectedUrl));
            });
        }
    }
}
