using System;
using System.Collections.Generic;
using NUnit.Framework;
using UrlTracker.Web.Controllers.Models;

namespace UrlTracker.Web.Tests.Controllers.Models
{
    public class AddRedirectRequestTests : ModelValidationTestBase<AddRedirectRequestTests>
    {
        public static IEnumerable<TestCaseData> TestCases()
        {
            yield return new TestCaseData(
                new AddRedirectRequest
                {
                    Is404 = true,
                    OldUrl = "https://example.com",
                    RedirectHttpCode = 301
                },
                Array.Empty<string>(),
                new[] { nameof(AddRedirectRequest.OldUrl) }
                ).SetName("TryValidateObject returns true on a minimal valid request model");

            yield return new TestCaseData(
                new AddRedirectRequest
                {
                    Is404 = false,
                    OldUrl = null,
                    RedirectHttpCode = 301
                },
                Array.Empty<string>(),
                new[] { nameof(AddRedirectRequest.OldUrl) }
                ).SetName("TryValidateObject returns true on a minimal valid request model");

            yield return new TestCaseData(
                new AddRedirectRequest
                {
                    Is404 = true,
                    OldUrl = null,
                    RedirectHttpCode = 301
                },
                new[] { nameof(AddRedirectRequest.OldUrl) },
                Array.Empty<string>()
                ).SetName("TryValidateObject returns false on a request model if Is404 is true but no old url is given");
        }

        [TestCaseSource(nameof(TestCases))]
        public void TryValidateObject_NormalFlow_ValidatesObject(AddRedirectRequest model, string[] errorsFor, string[] noErrorsFor)
        {
            base.TryValidateObject_NormalFlow_ValidatesObject(model, errorsFor, noErrorsFor);
        }
    }
}
