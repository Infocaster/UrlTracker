﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Resources.Testing;
using UrlTracker.Web.Processing;

namespace UrlTracker.Web.Tests.Processing
{
    public class RequestInterceptFilterCollectionTests : TestBase
    {
        private RequestInterceptFilterCollection? _testSubject;

        public override void SetUp()
        {
            _testSubject = new RequestInterceptFilterCollection(() => new List<IRequestInterceptFilter> { RequestInterceptFilter });
        }

        [TestCase(true, true, TestName = "EvaluateCandidateAsync returns true if all filters return true")]
        [TestCase(false, false, TestName = "EvaluateCandidateAsync returns false if any filter returns false")]
        public async Task EvaluateCandidateAsync_NormalFlow_ReturnsResult(bool output, bool expected)
        {
            // arrange
            RequestInterceptFilterMock!.Setup(obj => obj.EvaluateCandidateAsync(It.IsAny<Url>()))
                                      .ReturnsAsync(output);

            // act
            var result = await _testSubject!.EvaluateCandidateAsync(Url.Parse("http://example.com"));

            // assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
