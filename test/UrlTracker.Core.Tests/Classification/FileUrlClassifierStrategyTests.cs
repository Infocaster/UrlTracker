using System;
using Moq;
using NUnit.Framework;
using UrlTracker.Core.Classification;
using UrlTracker.Core.Database.Entities;
using UrlTracker.Core.Models;
using UrlTracker.Resources.Testing.Logging;

namespace UrlTracker.Core.Tests.Classification
{
    public class FileUrlClassifierStrategyTests
    {
        private FileUrlClassifierStrategy _testSubject = null!;
        private Mock<IRedactionScoreService> _redactionScoreServiceMock;

        [SetUp]
        public void SetUp()
        {
            _redactionScoreServiceMock = new Mock<IRedactionScoreService>();
            _redactionScoreServiceMock.Setup(obj => obj.Get(It.Is<Guid>(k => k == Defaults.DatabaseSchema.RedactionScores.File))).Returns(new RedactionScoreEntity());
            _testSubject = new FileUrlClassifierStrategy(_redactionScoreServiceMock.Object, new VoidLogger<FileUrlClassifierStrategy>());
        }

        [TestCase("http://example.com/file.pdf", false, TestName = "When the url is a file, this method returns a file classifier")]
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
