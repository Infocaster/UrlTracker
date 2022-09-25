using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using NUnit.Framework;
using UrlTracker.Backoffice.UI.Controllers.ActionFilters;
using UrlTracker.Backoffice.UI.Controllers.Models;
using UrlTracker.Resources.Testing;

namespace UrlTracker.Backoffice.UI.Tests.Controllers.ActionFilters
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
            _actionExecutingContext = new ActionExecutingContext(_actionContext, new List<IFilterMetadata>(), new Dictionary<string, object?>(), new object());
        }

        [TestCase(null, null, TestName = "OnActionExecuting doesn't touch urls that are null")]
        [TestCase("lorem/ipsum", "/lorem/ipsum", TestName = "OnActionExecuting prepends a slash if the url seems relative without a slash")]
        [TestCase("https://example.com", "https://example.com", TestName = "OnActionExecuting doesn't touch absolute urls")]
        [TestCase("/lorem/ipsum", "/lorem/ipsum", TestName = "OnActionExecuting doesn't touch relative urls with a leading slash")]
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
