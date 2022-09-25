using NUnit.Framework;
using UrlTracker.Backoffice.UI.Controllers.Models;

namespace UrlTracker.Backoffice.UI.Tests.Controllers.Models
{
    public class RedirectRequestBaseTests : ModelValidationTestBase<RedirectRequestBase>
    {
        public static IEnumerable<TestCaseData> TestCases()
        {
            yield return new TestCaseData(
                new TestRedirectRequest
                {
                    OldUrl = "http://example.com",
                    RedirectHttpCode = 301,
                    RedirectNodeId = 1000,
                    RedirectRootNodeId = 1001
                },
                Array.Empty<string>(),
                Array.Empty<string>(),
                true
                ).SetName("TryValidateObject returns true on a minimal valid redirect with old url");

            yield return new TestCaseData(
                new TestRedirectRequest
                {
                    OldRegex = "[0-9]{6}",
                    RedirectHttpCode = 301,
                    RedirectNodeId = 1000,
                    RedirectRootNodeId = 1001
                },
                Array.Empty<string>(),
                Array.Empty<string>(),
                true
                ).SetName("TryValidateObject returns true on a minimal valid redirect with old regex");

            yield return new TestCaseData(
                new TestRedirectRequest
                {
                    OldUrl = "http://example.com",
                    RedirectHttpCode = 301,
                    RedirectUrl = "http://example.com/lorem",
                    RedirectRootNodeId = 1001
                },
                Array.Empty<string>(),
                Array.Empty<string>(),
                true
                ).SetName("TryValidateObject returns true on a minimal valid redirect with target url");

            yield return new TestCaseData(
                new TestRedirectRequest
                {
                    RedirectHttpCode = 301,
                    RedirectUrl = "http://example.com/lorem",
                    RedirectRootNodeId = 1001
                },
                new[] { nameof(RedirectRequestBase.OldUrl), nameof(RedirectRequestBase.OldRegex) },
                Array.Empty<string>(),
                false).SetName("TryValidateObject returns false on a redirect with neither old url nor old regex");

            yield return new TestCaseData(
                new TestRedirectRequest
                {
                    OldUrl = "http://example.com",
                    RedirectHttpCode = 404,
                    RedirectUrl = "http://example.com/lorem",
                    RedirectRootNodeId = 1001
                },
                new[] { nameof(RedirectRequestBase.RedirectHttpCode) },
                Array.Empty<string>(),
                false).SetName("TryValidateObject returns false on a redirect when http code is out of range");

            yield return new TestCaseData(
                new TestRedirectRequest
                {
                    OldUrl = "http://example.com",
                    RedirectHttpCode = 301,
                    RedirectRootNodeId = 1001
                },
                new[] { nameof(RedirectRequestBase.RedirectUrl), nameof(RedirectRequestBase.RedirectNodeId) },
                Array.Empty<string>(),
                false).SetName("TryValidateObject returns false on a redirect when neither redirect url nor redirect node id is given.");

            yield return new TestCaseData(
                new TestRedirectRequest
                {
                    OldUrl = "http://example.com",
                    RedirectHttpCode = 301,
                    RedirectUrl = "http://example.com/lorem"
                },
                new[] { nameof(RedirectRequestBase.RedirectRootNodeId) },
                Array.Empty<string>(),
                false).SetName("TryValidateObject returns false on a redirect when root node id is not given.");
        }

        [TestCaseSource(nameof(TestCases))]
        public void TryValidateObject_NormalFlow_ValidatesObject(RedirectRequestBase model, string[] errorsFor, string[] noErrorsFor, bool mustBeValid)
        {
            base.TryValidateObject_NormalFlow_ValidatesObject(model, errorsFor, noErrorsFor, mustBeValid);
        }
    }

    public class TestRedirectRequest : RedirectRequestBase
    { }
}
