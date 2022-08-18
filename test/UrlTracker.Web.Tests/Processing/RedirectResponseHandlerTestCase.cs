using NUnit.Framework;
using UrlTracker.Core.Models;

namespace UrlTracker.Web.Tests.Processing
{
    public class RedirectResponseHandlerTestCase
    {
        public Redirect Redirect { get; set; }
        public int InitialStatusCode { get; set; }
        public int ExpectedStatusCode { get; set; }
        public string ExpectedUrl { get; set; }
        public string InitialUrl { get; set; }

        public TestCaseData ToTestCase(string name)
        {
            return new TestCaseData(Redirect, InitialStatusCode, ExpectedStatusCode, InitialUrl, ExpectedUrl).SetName(name);
        }
    }
}
