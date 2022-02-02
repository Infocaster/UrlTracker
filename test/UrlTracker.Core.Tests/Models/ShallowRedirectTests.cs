using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using NUnit.Framework;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Models;
using UrlTracker.Resources.Testing.Objects;

namespace UrlTracker.Core.Tests.Models
{
    public class ShallowRedirectTests
    {
        public static IEnumerable<TestCaseData> ValidateTestCases()
        {
            yield return new TestCaseData(
                new ShallowRedirect
                {
                    TargetRootNode = new TestPublishedContent(),
                    TargetStatusCode = HttpStatusCode.Redirect,
                    SourceUrl = Url.Parse("http://example.com"),
                    TargetUrl = "http://example.com/lorem"
                },
                true,
                null).SetName("Model is valid if it has a source url and a target url");
            yield return new TestCaseData(
                new ShallowRedirect
                {
                    TargetRootNode = new TestPublishedContent(),
                    TargetNode = new TestPublishedContent(),
                    TargetStatusCode = HttpStatusCode.Redirect,
                    SourceRegex = "lorem ipsum"
                },
                true,
                null).SetName("Model is valid if it has a source regex and a target node");
            yield return new TestCaseData(
                new ShallowRedirect
                {
                    TargetRootNode = new TestPublishedContent(),
                    TargetStatusCode = HttpStatusCode.OK,
                    TargetNode = new TestPublishedContent(),
                    SourceUrl = Url.Parse("http://example.com")
                },
                false,
                new[] { "TargetStatusCode" }).SetName("Model is invalid if the http status code is out of range");
            yield return new TestCaseData(
                new ShallowRedirect
                {
                    TargetRootNode = null,
                    TargetStatusCode = HttpStatusCode.Redirect,
                    TargetNode = new TestPublishedContent(),
                    SourceUrl = Url.Parse("http://example.com")
                },
                false,
                new[] { "TargetRootNode" }).SetName("Model is invalid if the root node is not defined");
            yield return new TestCaseData(
                new ShallowRedirect
                {
                    TargetRootNode = new TestPublishedContent(),
                    TargetStatusCode = HttpStatusCode.Redirect,
                    SourceUrl = Url.Parse("http://example.com")
                },
                false,
                new[] { "TargetNode", "TargetUrl" }).SetName("Model is invalid if no target is specified");
            yield return new TestCaseData(
                new ShallowRedirect
                {
                    TargetRootNode = new TestPublishedContent(),
                    TargetStatusCode = HttpStatusCode.Redirect,
                    TargetNode = new TestPublishedContent()
                },
                false,
                new[] { "SourceUrl", "SourceRegex" }).SetName("Model is invalid if no source condition is specified");

        }

        [TestCaseSource(nameof(ValidateTestCases))]
        public void Redirect_NormalFlow_HasProperValidation(ShallowRedirect input, bool isValid, IEnumerable<string> expectedInvalidProperties)
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
