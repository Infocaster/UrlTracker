using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using NUnit.Framework;
using UrlTracker.Core.Models;
using UrlTracker.Resources.Testing.Objects;

namespace UrlTracker.Core.Tests.Models
{
    public class RedirectTests
    {
        public static IEnumerable<TestCaseData> ValidateTestCases()
        {
            yield return new TestCaseData(
                new Redirect
                {
                    TargetRootNode = TestPublishedContent.Create(1),
                    TargetStatusCode = HttpStatusCode.Redirect,
                    SourceUrl = "http://example.com",
                    TargetUrl = "http://example.com/lorem"
                },
                true,
                null).SetName("Model is valid if it has a source url and a target url");
            yield return new TestCaseData(
                new Redirect
                {
                    TargetRootNode = TestPublishedContent.Create(1),
                    TargetNode = TestPublishedContent.Create(2),
                    TargetStatusCode = HttpStatusCode.Redirect,
                    SourceRegex = "lorem ipsum"
                },
                true,
                null).SetName("Model is valid if it has a source regex and a target node");
            yield return new TestCaseData(
                new Redirect
                {
                    TargetRootNode = TestPublishedContent.Create(1),
                    TargetStatusCode = HttpStatusCode.OK,
                    TargetNode = TestPublishedContent.Create(2),
                    SourceUrl = "http://example.com"
                },
                false,
                new[] { "TargetStatusCode" }).SetName("Model is invalid if the http status code is out of range");
            yield return new TestCaseData(
                new Redirect
                {
                    TargetRootNode = null,
                    TargetStatusCode = HttpStatusCode.Redirect,
                    TargetNode = TestPublishedContent.Create(1),
                    SourceUrl = "http://example.com"
                },
                false,
                new[] { "TargetRootNode" }).SetName("Model is invalid if the root node is not defined");
            yield return new TestCaseData(
                new Redirect
                {
                    TargetRootNode = TestPublishedContent.Create(1),
                    TargetStatusCode = HttpStatusCode.Redirect,
                    SourceUrl = "http://example.com"
                },
                false,
                new[] { "TargetNode", "TargetUrl" }).SetName("Model is invalid if no target is specified");
            yield return new TestCaseData(
                new Redirect
                {
                    TargetRootNode = TestPublishedContent.Create(1),
                    TargetStatusCode = HttpStatusCode.Redirect,
                    TargetNode = TestPublishedContent.Create(2)
                },
                false,
                new[] { "SourceUrl", "SourceRegex" }).SetName("Model is invalid if no source condition is specified");

        }

        [TestCaseSource(nameof(ValidateTestCases))]
        public void Redirect_NormalFlow_HasProperValidation(Redirect input, bool isValid, IEnumerable<string> expectedInvalidProperties)
        {
            // arrange
            ValidationContext ctx = new ValidationContext(input);
            List<ValidationResult> validationResults = new List<ValidationResult>();

            // act
            var result = Validator.TryValidateObject(input, ctx, validationResults, true);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(isValid));
                if (expectedInvalidProperties?.Any() == true)
                {
                    var invalidProperties = validationResults.SelectMany(vr => vr.MemberNames);
                    Assert.That(invalidProperties, Is.EquivalentTo(expectedInvalidProperties));
                }
            });
        }
    }
}
