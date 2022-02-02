using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace UrlTracker.E2eTests.Redirect
{
    public class RedirectTestModelsRedirect
    {
        public string Culture { get; set; }
        public bool Force { get; set; }
        public int? Id { get; set; }
        public string Notes { get; set; }
        public bool PassThroughQueryString { get; set; }
        public string SourceRegex { get; set; }
        public string SourceUrl { get; set; }
        public string TargetNodeName { get; set; }
        public string TargetRootNodeName { get; set; }
        public int TargetStatusCode { get; set; }
        public string TargetUrl { get; set; }
    }

    public class RedirectTestCase
    {
        public IEnumerable<RedirectTestModelsRedirect> Redirects { get; set; }
        public Uri RequestUrl { get; set; }
        public int ExpectedResponseStatusCode { get; set; }
        public Uri ExpectedResponseUri { get; set; }

        public TestCaseData ToTestCase(string name, string description)
        {
            return new TestCaseData(Redirects, RequestUrl, ExpectedResponseStatusCode, ExpectedResponseUri).SetDescription(description).SetName(name);
        }
    }
}
