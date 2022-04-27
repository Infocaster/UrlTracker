using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UrlTracker.Web.Controllers.Models;

namespace UrlTracker.Web.Tests.Controllers.Models
{
    public class IdRequestTests : ModelValidationTestBase<IIdRequest>
    {
        public static IEnumerable<TestCaseData> TestCases()
        {
            return CreateTestCaseForEveryType(obj =>
            {
                obj.Id = 1;

                return new TestCaseData(
                    obj,
                    Array.Empty<string>(),
                    new[] { nameof(IIdRequest.Id) }
                    ).SetName($"{obj.GetType().Name}.TryValidateObject returns true on a minimal valid request model");
            }).Concat(CreateTestCaseForEveryType(obj =>
            {
                return new TestCaseData(
                    obj,
                    new[] { nameof(IIdRequest.Id) },
                    Array.Empty<string>()
                    ).SetName($"{obj.GetType().Name}.TryValidateObject returns false on a request model if id is not given");
            }));
        }

        [TestCaseSource(nameof(TestCases))]
        public void TryValidateObject_NormalFlow_ValidatesObject(IIdRequest model, string[] errorsFor, string[] noErrorsFor)
        {
            base.TryValidateObject_NormalFlow_ValidatesObject(model, errorsFor, noErrorsFor);
        }
    }
}
