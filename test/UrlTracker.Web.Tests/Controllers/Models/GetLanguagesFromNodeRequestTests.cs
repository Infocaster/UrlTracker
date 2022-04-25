using System;
using System.Collections.Generic;
using NUnit.Framework;
using UrlTracker.Web.Controllers.Models;

namespace UrlTracker.Web.Tests.Controllers.Models
{
    public class GetLanguagesFromNodeRequestTests : ModelValidationTestBase<GetLanguagesFromNodeRequest>
    {
        public static IEnumerable<TestCaseData> TestCases()
        {
            yield return new TestCaseData(
                new GetLanguagesFromNodeRequest
                {
                    NodeId = 1
                },
                Array.Empty<string>(),
                new[] { nameof(GetLanguagesFromNodeRequest.NodeId) }
                ).SetName("TryValidateObject returns true on a minimal valid request model");

            yield return new TestCaseData(
                new GetLanguagesFromNodeRequest { },
                new[] { nameof(GetLanguagesFromNodeRequest.NodeId) },
                Array.Empty<string>()
                ).SetName("TryValidateObject returns false on a request model if node id is not given");
        }

        [TestCaseSource(nameof(TestCases))]
        public void TryValidateObject_NormalFlow_ValidatesObject(GetLanguagesFromNodeRequest model, string[] errorsFor, string[] noErrorsFor)
        {
            base.TryValidateObject_NormalFlow_ValidatesObject(model, errorsFor, noErrorsFor);
        }
    }
}
