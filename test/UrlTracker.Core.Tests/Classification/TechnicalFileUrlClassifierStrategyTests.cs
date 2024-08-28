using System;
using Moq;
using NUnit.Framework;
using UrlTracker.Core.Classification;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Models;
using UrlTracker.Resources.Testing.Logging;

namespace UrlTracker.Core.Tests.Classification
{
    public class TechnicalFileUrlClassifierStrategyTests
    {
        private TechnicalFileUrlClassifierStrategy _testSubject = null!;
        private Mock<IRedactionScoreService> _redactionScoreServiceMock;

        [SetUp]
        public void SetUp()
        {
            _redactionScoreServiceMock = new Mock<IRedactionScoreService>();
            _redactionScoreServiceMock.Setup(obj => obj.Get(It.Is<Guid>(k => k == Defaults.DatabaseSchema.RedactionScores.TechnicalFile))).Returns(new RedactionScoreEntity());
            _testSubject = new TechnicalFileUrlClassifierStrategy(_redactionScoreServiceMock.Object, new VoidLogger<TechnicalFileUrlClassifierStrategy>());
        }

        [TestCase("http://example.com/style.css", false, TestName = "When the url is a style sheet, this method returns a technical file classifier")]
        [TestCase("http://example.com/image.jpeg", true, TestName = "When the url is a non-technical file, this method returns null")]
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
