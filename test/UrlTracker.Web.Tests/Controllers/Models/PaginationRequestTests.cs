using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UrlTracker.Web.Controllers.Models;

namespace UrlTracker.Web.Tests.Controllers.Models
{
    public class PaginationRequestTests : ModelValidationTestBase<IPaginationRequest>
    {
        public static IEnumerable<TestCaseData> TestCases()
        {
            return CreateTestCaseForEveryType(obj =>
            {
                obj.Skip = 0;
                obj.Amount = 1;
                return new TestCaseData(
                    obj,
                    Array.Empty<string>(),
                    new[] { nameof(IPaginationRequest.Skip), nameof(IPaginationRequest.Amount) }
                    ).SetName($"{obj.GetType().Name}.TryValidateObject returns no validation errors on a minimal valid request object");
            }).Concat(CreateTestCaseForEveryType(obj =>
            {
                obj.Skip = 0;
                return new TestCaseData(
                    obj,
                    new[] { nameof(IPaginationRequest.Amount) },
                    new[] { nameof(IPaginationRequest.Skip) }
                    ).SetName($"{obj.GetType().Name}.TryValidateObject returns validation error on a request object if amount is not given");
            }).Concat(CreateTestCaseForEveryType(obj =>
            {
                obj.Amount = 1;
                return new TestCaseData(
                    obj,
                    new[] { nameof(IPaginationRequest.Skip) },
                    new[] { nameof(IPaginationRequest.Amount) }
                    ).SetName($"{obj.GetType().Name}.TryValidateObject returns validation error on a request object if skip is not given");
            })));
        }

        [TestCaseSource(nameof(TestCases))]
        public void TryValidateObject_NormalFlow_ValidatesObject(IPaginationRequest model, string[] errorsFor, string[] noErrorsFor)
        {
            base.TryValidateObject_NormalFlow_ValidatesObject(model, errorsFor, noErrorsFor);
        }
    }
}
