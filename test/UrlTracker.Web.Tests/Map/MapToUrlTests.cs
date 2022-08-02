using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Umbraco.Core.Mapping;
using Umbraco.Core.Models.PublishedContent;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Models;
using UrlTracker.Resources.Testing;
using UrlTracker.Resources.Testing.Mocks;
using UrlTracker.Resources.Testing.Objects;
using UrlTracker.Web.Map;

namespace UrlTracker.Web.Tests.Map
{
    public class MapToUrlTests : TestBase
    {
        protected override ICollection<IMapDefinition> CreateMappers()
        {
            return new IMapDefinition[]
            {
                new RedirectMap(UrlTrackerSettings, DomainProvider, UmbracoContextFactoryAbstractionMock.UmbracoContextFactory)
            };
        }

        public static IEnumerable<TestCaseData> TestCases()
        {
            yield return new MapTestCase
            {
                AppendPortNumber = false,
                IncomingUrl = "https://example.com",
                ExpectedUrl = Url.Parse("http://example.com/lorem"),
                Redirect = new ShallowRedirect
                {
                    TargetUrl = "http://example.com/lorem"
                }
            }.ToTestCase("MapToUrl redirect to absolute url returns absolute url");

            yield return new MapTestCase
            {
                AppendPortNumber = false,
                IncomingUrl = "https://example.com",
                ExpectedUrl = Url.Parse("https://example.com/lorem"),
                Redirect = new ShallowRedirect
                {
                    TargetUrl = "/lorem"
                }
            }.ToTestCase("MapToUrl redirect to relative url returns absolute url");

            yield return new MapTestCase
            {
                AppendPortNumber = false,
                IncomingUrl = "https://example.com",
                ExpectedUrl = Url.Parse("https://example.com/lorem"),
                ContentUrls = new List<KeyValuePair<int, string>>
                {
                    new KeyValuePair<int, string>(2, "https://example.com/lorem")
                },
                Redirect = new ShallowRedirect
                {
                    TargetRootNode = TestPublishedContent.Create(1, PublishedItemType.Content),
                    TargetNode = TestPublishedContent.Create(2, PublishedItemType.Content)
                }
            }.ToTestCase("MapToUrl redirect to node returns node url");

            yield return new MapTestCase
            {
                AppendPortNumber = false,
                IncomingUrl = "https://urltracker.ic",
                ExpectedUrl = Url.Parse("https://urltracker.ic/lorem"),
                ContentUrls = new List<KeyValuePair<int, string>>
                {
                    new KeyValuePair<int, string>(2, "https://example.com/lorem")
                },
                Domains = DomainCollection.Create(new List<Domain>
                {
                    new Domain(0, 1, "example com", "nl", Url.Parse("https://urltracker.ic/"))
                }),
                Redirect = new ShallowRedirect
                {
                    TargetRootNode = TestPublishedContent.Create(1, PublishedItemType.Content),
                    TargetNode = TestPublishedContent.Create(2, PublishedItemType.Content),
                    Culture = "nl"
                }
            }.ToTestCase("MapToUrl redirect to node returns node url with root host");

            yield return new MapTestCase
            {
                AppendPortNumber = false,
                IncomingUrl = "https://example.com?dolor=sit",
                ExpectedUrl = Url.Parse("https://example.com/lorem?dolor=sit"),
                Redirect = new ShallowRedirect
                {
                    TargetUrl = "/lorem",
                    PassThroughQueryString = true,
                }
            }.ToTestCase("MapToUrl keep query string enabled keeps the query string");

            yield return new MapTestCase
            {
                AppendPortNumber = false,
                IncomingUrl = "https://example.com",
                ExpectedUrl = null,
                Redirect = new ShallowRedirect()
            }.ToTestCase("MapToUrl returns null when target is empty");

            yield return new MapTestCase
            {
                AppendPortNumber = true,
                IncomingUrl = "http://example.com:81",
                ExpectedUrl = Url.Parse("http://example.com:81/lorem"),
                Redirect = new ShallowRedirect
                {
                    TargetNode = TestPublishedContent.Create(1, PublishedItemType.Content),
                    TargetRootNode = TestPublishedContent.Create(2, PublishedItemType.Content)
                },
                ContentUrls = new[]
                {
                    new KeyValuePair<int, string>(1, "http://example.com/lorem")
                }
            }.ToTestCase("MapToUrl appends port number when configuration is true");

            yield return new MapTestCase
            {
                AppendPortNumber = true,
                IncomingUrl = "http://example.com:80",
                ExpectedUrl = Url.Parse("http://example.com/lorem"),
                Redirect = new ShallowRedirect
                {
                    TargetNode = TestPublishedContent.Create(1, PublishedItemType.Content),
                    TargetRootNode = TestPublishedContent.Create(2, PublishedItemType.Content)
                },
                ContentUrls = new[]
                {
                    new KeyValuePair<int, string>(1, "http://example.com/lorem")
                }
            }.ToTestCase("MapToUrl does not append port number if port is 80");

            yield return new MapTestCase
            {
                AppendPortNumber = false,
                IncomingUrl = "http://example.com/lorem",
                ExpectedUrl = Url.Parse("http://urltracker.ic/ipsum"),
                Redirect = new ShallowRedirect
                {
                    TargetNode = TestPublishedContent.Create(1, PublishedItemType.Content)
                },
                ContentUrls = new[]
                {
                    new KeyValuePair<int, string>(1, "http://urltracker.ic/ipsum")
                },
                Domains = DomainCollection.Create(new[]
                {
                    new Domain(1000, 2, "example com", "nl-nl", Url.Parse("http://example.com"))
                })
            }.ToTestCase("MapToUrl does not throw exceptions when root node is null");
        }

        [TestCaseSource(nameof(TestCases))]
        public void MapToUrl_NormalFlow_MapsRedirectToUrl(string incomingUrl, IEnumerable<KeyValuePair<int, string>> contentUrls, ShallowRedirect redirect, DomainCollection domains, bool appendPortNumber, Url expectedUrl)
        {
            // arrange
            HttpContextMock.RequestMock.SetupGet(obj => obj.Url).Returns(new Uri(incomingUrl));
            UrlTrackerSettings.Value = new UrlTrackerSettings(false, false, true, false, appendPortNumber, false, 5000, true, 60 * 48, true, new List<string>());
            if (contentUrls?.Any() == true)
            {
                foreach (var pair in contentUrls) UmbracoContextFactoryAbstractionMock.CrefMock.Setup(obj => obj.GetUrl(It.Is<IPublishedContent>(o => o.Id == pair.Key), It.IsAny<UrlMode>(), It.IsAny<string>())).Returns(pair.Value);
            }
            if (domains?.Any() == true)
            {
                DomainProviderMock.Setup(obj => obj.GetDomains()).Returns(domains);
            }

            // act
            var result = Mapper.MapToUrl(redirect, HttpContextMock.Context);

            // assert
            Assert.That(result, Is.EqualTo(expectedUrl));
        }
    }
}
