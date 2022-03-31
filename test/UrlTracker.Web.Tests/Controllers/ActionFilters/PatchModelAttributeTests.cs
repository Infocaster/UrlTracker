using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using NUnit.Framework;
using UrlTracker.Resources.Testing;
using UrlTracker.Web.Controllers.ActionFilters;
using UrlTracker.Web.Controllers.Models;

namespace UrlTracker.Web.Tests.Controllers.ActionFilters
{
    public class PatchModelAttributeTests : TestBase
    {
        private PatchModelAttribute? _testSubject;
        private ActionContext? _actionContext;
        private ActionExecutingContext? _actionExecutingContext;

        [SetUp]
        public override void SetUp()
        {
            _testSubject = new PatchModelAttribute();
            _actionContext = new ActionContext(HttpContextMock!.Context, new RouteData(), new ActionDescriptor());
            _actionExecutingContext = new ActionExecutingContext(_actionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), new object());
        }


        public static IEnumerable<TestCaseData> TestCases()
        {
            yield return new TestCaseData(null, null).SetName("OnActionExecuting doesn't touch urls that are null");
            yield return new TestCaseData("lorem/ipsum", "/lorem/ipsum").SetName("OnActionExecuting prepends a slash if the url seems relative without a slash");
            yield return new TestCaseData("https://example.com", "https://example.com").SetName("OnActionExecuting doesn't touch absolute urls");
            yield return new TestCaseData("/lorem/ipsum", "/lorem/ipsum").SetName("OnActionExecuting doesn't touch relative urls with a leading slash");
        }

        [TestCaseSource(nameof(TestCases))]
        public void OnActionExecuting_IncomingUrls_PatchesModels(string inputUrl, string expected)
        {
            // arrange
            AddRedirectRequest input = new() { OldUrl = inputUrl, RedirectUrl = inputUrl };
            _actionExecutingContext!.ActionArguments.Add("test", input);

            // act
            _testSubject!.OnActionExecuting(_actionExecutingContext);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(input.OldUrl, Is.EqualTo(expected));
                Assert.That(input.RedirectUrl, Is.EqualTo(expected));
            });
        }
    }
}
