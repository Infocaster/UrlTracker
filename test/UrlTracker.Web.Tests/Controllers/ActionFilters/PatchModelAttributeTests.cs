using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using NUnit.Framework;
using UrlTracker.Resources.Testing.Objects;
using UrlTracker.Web.Controllers.ActionFilters;
using UrlTracker.Web.Controllers.Models;

namespace UrlTracker.Web.Tests.Controllers.ActionFilters
{
    public class PatchModelAttributeTests
    {
        private PatchModelAttribute _testSubject;
        private TestActionDescriptor _actionDescriptor;
        private HttpActionContext _actionContext;
        private TestParameterDescriptor _parameterDescriptor;

        [SetUp]
        public void SetUp()
        {
            _testSubject = new PatchModelAttribute();
            _parameterDescriptor = new TestParameterDescriptor("test", typeof(object));
            _actionDescriptor = new TestActionDescriptor("test", typeof(IHttpActionResult));
            _actionDescriptor.Parameters.Add(_parameterDescriptor);
            _actionContext = new HttpActionContext
            {
                ControllerContext = new HttpControllerContext { Request = new HttpRequestMessage() },
                Response = new HttpResponseMessage(),
                ActionDescriptor = _actionDescriptor,
            };
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
            AddRedirectRequest input = new AddRedirectRequest { OldUrl = inputUrl, RedirectUrl = inputUrl };
            _actionContext.ActionArguments.Add("test", input);

            // act
            _testSubject.OnActionExecuting(_actionContext);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That(input.OldUrl, Is.EqualTo(expected));
                Assert.That(input.RedirectUrl, Is.EqualTo(expected));
            });
        }
    }
}
