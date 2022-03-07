// using System.Collections.Generic;
// using System.Net;
// using System.Net.Http;
// using System.Web.Http;
// using System.Web.Http.Controllers;
// using NUnit.Framework;
// using UrlTracker.Core;
// using UrlTracker.Resources.Testing.Objects;
// using UrlTracker.Web.Controllers.ActionFilters;

// namespace UrlTracker.Web.Tests.Controllers.ActionFilters
// {
//     public class ValidateModelAttributeTests
//     {
//         private ValidateModelAttribute _testSubject;
//         private TestActionDescriptor _actionDescriptor;
//         private HttpActionContext _actionContext;
//         private TestParameterDescriptor _parameterDescriptor;

//         [SetUp]
//         public void SetUp()
//         {
//             _testSubject = new ValidateModelAttribute();
//             _parameterDescriptor = new TestParameterDescriptor("test", typeof(object));
//             _actionDescriptor = new TestActionDescriptor("test", typeof(IHttpActionResult));
//             _actionDescriptor.Parameters.Add(_parameterDescriptor);
//             _actionContext = new HttpActionContext
//             {
//                 ControllerContext = new HttpControllerContext { Request = new HttpRequestMessage() },
//                 Response = new HttpResponseMessage(),
//                 ActionDescriptor = _actionDescriptor,
//             };
//         }

//         [TestCase(TestName = "OnActionExecuting writes 400 to response if the model state is invalid")]
//         public void OnActionExecuting_ModelHasErrors_ReturnsModelError()
//         {
//             // arrange
//             _actionContext.ModelState.AddModelError("test", "This property is required");

//             // act
//             _testSubject.OnActionExecuting(_actionContext);

//             // assert
//             Assert.Multiple(() =>
//             {
//                 Assert.That(_actionContext.Response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
//                 Assert.That(_actionContext.Response.Content.ReadAsStringAsync().Result, Contains.Substring("This property is required"));
//             });
//         }

//         public static IEnumerable<TestCaseData> MissingParametersTestCases()
//         {
//             yield return new TestCaseData(new FromBodyAttribute(), Defaults.Validation.BodyContentMissing)
//                 .SetName("OnActionExecuting writes 400 to response if body content is missing");
//             yield return new TestCaseData(new FromUriAttribute(), Defaults.Validation.QueryParamsMissing)
//                 .SetName("OnActionExecuting writes 400 to response if query parameters are missing");
//         }

//         [TestCaseSource(nameof(MissingParametersTestCases))]
//         public void OnActionExecuting_MissingParameters_ReturnsModelError(ParameterBindingAttribute attribute, string expectedMessage)
//         {
//             // arrange
//             _actionContext.ActionArguments.Add("test", null);
//             _parameterDescriptor.ParameterBinderAttribute = attribute;

//             // act
//             _testSubject.OnActionExecuting(_actionContext);

//             // assert
//             Assert.Multiple(() =>
//             {
//                 Assert.That(_actionContext.Response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
//                 Assert.That(_actionContext.Response.Content.ReadAsStringAsync().Result, Contains.Substring(expectedMessage));
//             });
//         }

//         [TestCase(TestName = "OnActionExecuting does nothing if all parameter models pass validation")]
//         public void OnActionExecuting_NormalFlow_DoesNothing()
//         {
//             // arrange
//             _actionContext.ActionArguments.Add("test", new object());

//             // act
//             _testSubject.OnActionExecuting(_actionContext);

//             // assert
//             Assert.That(_actionContext.Response.StatusCode, Is.Not.EqualTo(HttpStatusCode.BadRequest));
//         }

//         [TestCase(TestName = "OnActionExecuting does nothing if an optional parameter has no assigned value")]
//         public void OnActionExecuting_ValueMissingForOptionalParameter_DoesNothing()
//         {
//             // arrange
//             _parameterDescriptor.SetIsOptional(true);

//             // act
//             _testSubject.OnActionExecuting(_actionContext);

//             // assert
//             Assert.That(_actionContext.Response.StatusCode, Is.Not.EqualTo(HttpStatusCode.BadRequest));
//         }
//     }
// }
