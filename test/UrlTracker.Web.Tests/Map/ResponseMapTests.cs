﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Moq;
using NUnit.Framework;
using Umbraco.Core.Mapping;
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
                CreateTestMap<Redirect, Url>(Url.Parse("http://example.com/lorem")),
                new ResponseMap(LocalizationService, HttpContextAccessorAbstraction)
            };
        }

        protected override HttpContextMock CreateHttpContextMock()
            => new HttpContextMock(new Uri("http://example.com"));

        public override void SetUp()
        {
            HttpContextAccessorAbstractionMock.Setup(obj => obj.HttpContext).Returns(HttpContextMock.Context);
        }

        [TestCase(TestName = "Map Redirect to RedirectViewModel")]
        public void Map_Redirect_RedirectViewModel()
        {
            // arrange
            var input = new Redirect
            {
                Culture = "nl-nl",
                Force = true,
                Id = 1000,
                Inserted = new DateTime(2022, 1, 26),
                Notes = "lorem ipsum",
                PassThroughQueryString = true,
                SourceRegex = "dolor sit",
                SourceUrl = Url.Parse("http://example.com/ipsum"),
                TargetNode = new TestPublishedContent { Id = 1001 },
                TargetRootNode = new TestPublishedContent { Id = 1002 },
                TargetStatusCode = HttpStatusCode.Redirect,
                TargetUrl = "http://example.com/dolor"
            };

            // act
            var result = Mapper.Map<RedirectViewModel>(input);

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
                Assert.That(result.RedirectPassThroughQueryString, Is.EqualTo(input.PassThroughQueryString));
                Assert.That(result.RedirectRootNodeId, Is.EqualTo(1002));
                Assert.That(result.RedirectUrl, Is.EqualTo(input.TargetUrl));
                Assert.That(result.Referrer, Is.Null);
                Assert.That(result.Remove404, Is.False);
            });
        }

        [TestCase(TestName = "Map RichNotFound to RedirectViewModel")]
        public void Map_RichNotFound_RedirectViewModel()
        {
            // arrange
            var input = new RichNotFound
            {
                Id = 1000,
                LatestOccurrence = new DateTime(2022, 1, 26),
                MostCommonReferrer = "http://example.com",
                Occurrences = 3,
                Url = "http://example.com/lorem"
            };

            // act
            var result = Mapper.Map<RedirectViewModel>(input);

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

        [TestCase(TestName = "Map RichNotFound to RedirectViewModel with query string")]
        public void Map_RichNotFound_RedirectViewModel_QueryString()
        {
            // arrange
            var input = new RichNotFound
            {
                Url = "http://example.com/?lorem=ipsum"
            };

            // act
            var result = Mapper.Map<RedirectViewModel>(input);

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
            var input = new UrlTrackerSettings(true, true, true, true, true, true);

            // act
            var result = Mapper.Map<GetSettingsResponse>(input);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.AppendPortNumber, Is.True);
                Assert.That(result.EnableLogging, Is.True);
                Assert.That(result.IsDisabled, Is.True);
                Assert.That(result.IsNotFoundTrackingDisabled, Is.True);
                Assert.That(result.TrackingDisabled, Is.True);
            });
        }

        [TestCase(TestName = "Map Domain to GetLanguagesFromNodeResponseLanguage")]
        public void Map_Domain_GetLanguagesFromNodeResponseLanguage()
        {
            // arrange
            LocalizationServiceMock.Setup(obj => obj.GetLanguageByIsoCode(It.IsAny<string>())).Returns((string isoCode) => new TestLanguage { IsoCode = isoCode, CultureName = "Dutch", Id = 1003 });
            var input = new Domain(1000, 1001, "lorem ipsum", "nl-nl", Url.Parse("http://example.com"));

            // act
            var result = Mapper.Map<GetLanguagesFromNodeResponseLanguage>(input);

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
            var result = Mapper.Map<GetRedirectsResponse>(input);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Entries.Count(), Is.EqualTo(input.Count()));
                Assert.That(result.NumberOfEntries, Is.EqualTo(input.Total));
            });
        }

        [TestCase(TestName = "Map RichNotFoundCollection to GetNotFoundsResponse")]
        public void Map_RichNotFoundCollection_GetNotFoundsResponse()
        {
            // arrange
            var input = RichNotFoundCollection.Create(new[] { new RichNotFound { Id = 1000, Url = "http://example.com" } }, 3);

            // act
            var result = Mapper.Map<GetNotFoundsResponse>(input);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Entries.Count(), Is.EqualTo(input.Count()));
                Assert.That(result.NumberOfEntries, Is.EqualTo(input.Total));
            });
        }
    }
}
