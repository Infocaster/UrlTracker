using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using UrlTracker.Core.Database;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Intercepting;
using UrlTracker.Core.Intercepting.Models;
using UrlTracker.Core.Models;
using UrlTracker.Resources.Testing.Logging;

namespace UrlTracker.Core.Tests.Intercepting
{
    public class StaticUrlRedirectMatcherTests
    {
        private Mock<IRedirectRepository> _redirectRepositoryMock;
        private StaticUrlRedirectInterceptor? _testSubject;

        [SetUp]
        public void SetUp()
        {
            _redirectRepositoryMock = new Mock<IRedirectRepository>();
            _testSubject = new StaticUrlRedirectInterceptor(_redirectRepositoryMock.Object, new StaticUrlProviderCollection(() => new List<IStaticUrlProvider> { new StaticUrlProvider() }), new VoidLogger<StaticUrlRedirectInterceptor>());
        }

        public static IEnumerable<TestCaseData> TestCases()
        {
            var result1 = new RedirectEntity(default, default, default, EntityStrategy.UrlSource("http://example.com/"), default);
            var result2 = new RedirectEntity(default, default, default, EntityStrategy.UrlSource("http://example.com/lorem"), default);

            yield return new TestCaseData(
                Array.Empty<IRedirect>(),
                null).SetName("InterceptAsync returns null if no match is found");

            yield return new TestCaseData(
                new IRedirect[] { result1, result2 },
                result1).SetName("InterceptAsync returns first result if any are found");
        }

        [TestCaseSource(nameof(TestCases))]
        public async Task GetMatchAsync_NormalFlow_ReturnsMatch(IRedirect[] output, IRedirect expected)
        {
            // arrange
            _redirectRepositoryMock!.Setup(obj => obj.GetAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(output);

            // act
            var result = await _testSubject!.InterceptAsync(Url.Parse("http://example.com"), new DefaultInterceptContext());

            // assert
            Assert.That(result?.Info, Is.EqualTo(expected));
        }
    }
}
