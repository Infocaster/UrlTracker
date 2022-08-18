using System.Collections.Generic;
using NUnit.Framework;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Models;

namespace UrlTracker.Web.Tests.Map
{
    public class MapTestCase
    {
        public string IncomingUrl { get; set; }
        public IEnumerable<KeyValuePair<int, string>> ContentUrls { get; set; }
        public Redirect Redirect { get; set; }
        public DomainCollection Domains { get; set; }
        public bool AppendPortNumber { get; set; }
        public Url ExpectedUrl { get; set; }

        public TestCaseData ToTestCase(string name)
        {
            return new TestCaseData(IncomingUrl, ContentUrls, Redirect, Domains, AppendPortNumber, ExpectedUrl).SetName(name);
        }
    }
}
