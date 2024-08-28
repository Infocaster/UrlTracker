using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Umbraco.Cms.Core.Models.PublishedContent;
using UrlTracker.Backoffice.UI.Controllers.Models;
using UrlTracker.Core.Models;
using UrlTracker.Resources.Testing.Objects;

namespace UrlTracker.Core.Tests.Models
{
    public class RedirectCultureValidationTests
    {
        public static IEnumerable<TestCaseData> ValidateTestCases()
        {
            yield return new TestCaseData(
                "nl",
                true).SetName("Culture is valid if it has a language code (2)");
            yield return new TestCaseData(
                "yi-001",
                true).SetName("Culture is valid if it has a language code (2) and a three-digit country code");
            yield return new TestCaseData(
                "en-US",
                true).SetName("Culture is valid if it has a language code (2) and a country/region code (2)");
            yield return new TestCaseData(
                "zh-Hant",
                true).SetName("Culture is valid if it has a language code (2) and a script tag");
            yield return new TestCaseData(
                "uz-Cyrl-UZ",
                true).SetName("Culture is valid if it has a language code (2), a script tag and country/region code (2)");
            yield return new TestCaseData(
                "uz-Cyrl-UZZ",
                false).SetName("Culture is invalid if it has more than two letters in country or region code");
            yield return new TestCaseData(
                "",
                true).SetName("Culture can be empty");
            yield return new TestCaseData(null,
                true).SetName("Culture can be null");
        }


        [TestCaseSource(nameof(ValidateTestCases))]
        public void Redirect_NormalFlow_HasProperValidation(string input, bool isValid)
        {
            // arrange
            var redirect = new Redirect
            {
                Source = new UrlSourceStrategy("http://example.com"),
                Target = new ContentPageTargetStrategy(TestPublishedContent.Create(1), input),
            };

            // act
            var result = Validator.TryValidateObject(redirect, new ValidationContext(redirect, null, null), null, true);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(isValid));
            });
        }
    }
}
