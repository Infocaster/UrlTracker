using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Moq;
using NUnit.Framework;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Models.PublishedContent;
using UrlTracker.Core.Configuration.Models;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Models;
using UrlTracker.Resources.Testing;
using UrlTracker.Resources.Testing.Mocks;
using UrlTracker.Resources.Testing.Objects;
using UrlTracker.Web.Controllers.Models;
using UrlTracker.Web.Map;

namespace UrlTracker.Web.Tests.Map
{
    public class ResponseMapTests : TestBase
    {
        protected override ICollection<IMapDefinition> CreateMappers()
        {
            return new IMapDefinition[]
            {
                new ResponseMap(LocalizationService, UmbracoContextFactoryAbstractionMock!.UmbracoContextFactory)
            };
        }

        protected override HttpContextMock CreateHttpContextMock()
            => new(new Uri("http://example.com"));

        public override void SetUp()
        {
            UmbracoContextFactoryAbstractionMock!.CrefMock.Setup(obj => obj.GetUrl(It.IsAny<IPublishedContent>(), It.IsAny<UrlMode>(), It.IsAny<string>()))
                .Returns("http://example.com/lorem");
        }

        [TestCase(TestName = "Map Redirect to RedirectViewModel with content")]
        public void Map_Redirect_RedirectViewModel_Content()
        {
            // arrange
            var input = new Redirect
            {
                Culture = "nl-nl",
                Force = true,
                Id = 1000,
                Inserted = new DateTime(2022, 1, 26),
                Notes = "lorem ipsum",
                RetainQuery = true,
                SourceRegex = "dolor sit",
                SourceUrl = "http://example.com/ipsum",
                TargetNode = TestPublishedContent.Create(1001, PublishedItemType.Content),
                TargetRootNode = TestPublishedContent.Create(1002, PublishedItemType.Content),
                TargetStatusCode = HttpStatusCode.Redirect,
                TargetUrl = "http://example.com/dolor"
            };

            // act
            var result = Mapper!.Map<RedirectViewModel>(input)!;

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.CalculatedRedirectUrl, Is.EqualTo("http://example.com/lorem"));
                Assert.That(result.Culture, Is.EqualTo(input.Culture));
                Assert.That(result.ForceRedirect, Is.EqualTo(input.Force));
                Assert.That(result.Id, Is.EqualTo(input.Id));
                Assert.That(result.Inserted, Is.EqualTo(input.Inserted));
                Assert.That(result.Is404, Is.False);
                Assert.That(result.Notes, Is.EqualTo(input.Notes));
                Assert.That(result.Occurrences, Is.Null);
                Assert.That(result.OldRegex, Is.EqualTo(input.SourceRegex));
                Assert.That(result.OldUrl, Is.EqualTo("http://example.com/ipsum"));
                Assert.That(result.OldUrlWithoutQuery, Is.EqualTo("http://example.com/ipsum"));
                Assert.That(result.RedirectHttpCode, Is.EqualTo(302));
                Assert.That(result.RedirectNodeId, Is.EqualTo(1001));
                Assert.That(result.RedirectPassThroughQueryString, Is.EqualTo(input.RetainQuery));
                Assert.That(result.RedirectRootNodeId, Is.EqualTo(1002));
                Assert.That(result.RedirectUrl, Is.EqualTo(input.TargetUrl));
                Assert.That(result.Referrer, Is.Null);
                Assert.That(result.Remove404, Is.False);
            });
        }

        [TestCase(TestName = "Map Redirect to RedirectViewModel without content")]
        public void Map_Redirect_RedirectViewModel_NoContent()
        {
            // arrange
            var input = new Redirect
            {
                Id = 1,
                SourceUrl = "http://example.com/?lorem=ipsum"
            };

            // act
            var result = Mapper!.Map<RedirectViewModel>(input)!;

            // assert
            Assert.That(result.OldUrlWithoutQuery, Is.EqualTo("http://example.com/"));
        }

        [TestCase(TestName = "Map ClientError to RedirectViewModel")]
        public void Map_ClientError_RedirectViewModel()
        {
            // arrange
            var input = new ClientError("http://example.com/lorem")
            {
                Id = 1000,
                LatestOccurrence = new DateTime(2022, 1, 26),
                MostCommonReferrer = "http://example.com",
                Occurrences = 3
            };

            // act
            var result = Mapper!.Map<RedirectViewModel>(input)!;

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.CalculatedRedirectUrl, Is.Null);
                Assert.That(result.Culture, Is.Null);
                Assert.That(result.ForceRedirect, Is.False);
                Assert.That(result.Id, Is.EqualTo(input.Id));
                Assert.That(result.Inserted, Is.EqualTo(input.LatestOccurrence));
                Assert.That(result.Is404, Is.True);
                Assert.That(result.Notes, Is.Null);
                Assert.That(result.Occurrences, Is.EqualTo(input.Occurrences));
                Assert.That(result.OldRegex, Is.Null);
                Assert.That(result.OldUrl, Is.EqualTo(input.Url));
                Assert.That(result.OldUrlWithoutQuery, Is.EqualTo(input.Url));
                Assert.That(result.RedirectHttpCode, Is.Zero);
                Assert.That(result.RedirectNodeId, Is.Null);
                Assert.That(result.RedirectPassThroughQueryString, Is.False);
                Assert.That(result.RedirectRootNodeId, Is.EqualTo(-1));
                Assert.That(result.RedirectUrl, Is.Null);
                Assert.That(result.Referrer, Is.EqualTo(input.MostCommonReferrer));
                Assert.That(result.Remove404, Is.False);
            });
        }

        [TestCase(TestName = "Map ClientError to RedirectViewModel with query string")]
        public void Map_ClientError_RedirectViewModel_QueryString()
        {
            // arrange
            var input = new ClientError("http://example.com/?lorem=ipsum");

            // act
            var result = Mapper!.Map<RedirectViewModel>(input)!;

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.OldUrlWithoutQuery, Is.EqualTo("http://example.com/"));
            });
        }

        [TestCase(TestName = "Map UrlTrackerSettings to GetSettingsResponse")]
        public void Map_UrlTrackerSettings_GetSettingsResponse()
        {
            // arrange
            var input = new UrlTrackerSettings
            {
                AppendPortNumber = true,
                HasDomainOnChildNode = true,
                IsDisabled = true,
                IsTrackingDisabled = true,
                IsNotFoundTrackingDisabled = true,
                LoggingEnabled = true
            };

            // act
            var result = Mapper!.Map<GetSettingsResponse>(input)!;

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.AppendPortNumber, Is.True);
                Assert.That(result.EnableLogging, Is.True);
                Assert.That(result.IsDisabled, Is.True);
                Assert.That(result.IsNotFoundTrackingDisabled, Is.True);
                Assert.That(result.TrackingDisabled, Is.True);
                Assert.That(result.BlockedUrlsList, Is.Not.Null);
            });
        }

        [TestCase(TestName = "Map Domain to GetLanguagesFromNodeResponseLanguage")]
        public void Map_Domain_GetLanguagesFromNodeResponseLanguage()
        {
            // arrange
            LocalizationServiceMock!.Setup(obj => obj.GetLanguageByIsoCode(It.IsAny<string>())).Returns((string isoCode) => TestLanguage.Create(1003, isoCode, "Dutch"));
            var input = new Domain(1000, 1001, "lorem ipsum", "nl-nl", Url.Parse("http://example.com"));

            // act
            var result = Mapper!.Map<GetLanguagesFromNodeResponseLanguage>(input)!;

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.CultureName, Is.EqualTo("Dutch"));
                Assert.That(result.Id, Is.EqualTo(1003));
                Assert.That(result.IsoCode, Is.EqualTo(input.LanguageIsoCode));
            });
        }

        [TestCase(TestName = "Map RedirectCollection to GetRedirectsResponse")]
        public void Map_RedirectCollection_GetRedirectsResponse()
        {
            // arrange
            var input = RedirectCollection.Create(new[] { new Redirect { Id = 1000 } }, 3);

            // act
            var result = Mapper!.Map<GetRedirectsResponse>(input)!;

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Entries.Count, Is.EqualTo(input.Count()));
                Assert.That(result.NumberOfEntries, Is.EqualTo(input.Total));
            });
        }

        [TestCase(TestName = "Map ClientErrorCollection to GetClientErrorsResponse")]
        public void Map_ClientErrorCollection_GetClientErrorsResponse()
        {
            // arrange
            var input = ClientErrorCollection.Create(new[] { new ClientError("http://example.com") { Id = 1000 } }, 3);

            // act
            var result = Mapper!.Map<GetNotFoundsResponse>(input)!;

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Entries.Count, Is.EqualTo(input.Count()));
                Assert.That(result.NumberOfEntries, Is.EqualTo(input.Total));
            });
        }
    }
}
