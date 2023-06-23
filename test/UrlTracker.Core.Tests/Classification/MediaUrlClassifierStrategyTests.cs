﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using UrlTracker.Core.Classification;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Resources.Testing;
using UrlTracker.Resources.Testing.Logging;

namespace UrlTracker.Core.Tests.Classification
{
    public class MediaUrlClassifierStrategyTests : TestBase
    {
        private MediaUrlClassifierStrategy _testSubject = null!;

        public override void SetUp()
        {
            RedactionScoreServiceMock.Setup(obj => obj.Get(It.Is<Guid>(k => k == Defaults.DatabaseSchema.RedactionScores.Media))).Returns(new RedactionScoreEntity());
            _testSubject = new MediaUrlClassifierStrategy(RedactionScoreService, new VoidLogger<MediaUrlClassifierStrategy>());
        }

        [TestCase("http://example.com/image.jpg", false, TestName = "When the url is an image, this method returns an image classifier")]
        [TestCase("http://example.com/file.docx", true, TestName = "When the url is a non-media file, this method returns null")]
        [TestCase("http://example.com/about-us", true, TestName = "When the url is not a file, this method returns null")]
        public void Classify_NormalFlow_ReturnsCorrectResult(string url, bool isNull)
        {
            // arrange
            var input = Url.Parse(url);

            // act
            var result = _testSubject.Classify(input);

            // assert
            Assert.That(result is null, Is.EqualTo(isNull));
        }
    }
}
