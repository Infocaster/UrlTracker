using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace UrlTracker.E2eTests.Redirect
{
    public static class RedirectTestCasesSource
    {
        public static IEnumerable<TestCaseData> CreateValueSets(Uri baseAddress)
        {
            if (!baseAddress.IsAbsoluteUri) throw new ArgumentException("Uri must be absolute");

            return new List<TestCaseData>
            {
                // a redirect with a relative url should match on a url with the relative url as path
                new RedirectTestCase
                {
                    Redirects = new List<RedirectTestModelsRedirect>
                    {
                        new RedirectTestModelsRedirect
                        {
                            Culture = "nl",
                            Force = false,
                            Id = 0,
                            Notes = "lorem",
                            PassThroughQueryString = true,
                            SourceRegex = null,
                            SourceUrl = "/dolor",
                            TargetNodeName = null,
                            TargetRootNodeName = "root",
                            TargetStatusCode = 301,
                            TargetUrl = "/lorem"
                        }
                    },
                    RequestUrl = new Uri(baseAddress, "/dolor"),
                    ExpectedResponseStatusCode = 301,
                    ExpectedResponseUri = new Uri(baseAddress, "/lorem")
                }.ToTestCase("Relative redirect rule", "A redirect with a relative url should match on a url with the relative url as path"),

                // a redirect with an absolute url should match on a url with that exact url
                new RedirectTestCase
                {
                    Redirects = new List<RedirectTestModelsRedirect>
                    {
                        new RedirectTestModelsRedirect
                        {
                            Culture = "nl",
                            Force = false,
                            Id = 0,
                            Notes = "lorem",
                            PassThroughQueryString = true,
                            SourceRegex = null,
                            SourceUrl = new Uri(baseAddress, "/dolor/").AbsoluteUri,
                            TargetNodeName = "lorem",
                            TargetRootNodeName = "root",
                            TargetStatusCode = 302,
                            TargetUrl = null
                        }
                    },
                    RequestUrl = new Uri(baseAddress, "/dolor"),
                    ExpectedResponseStatusCode = 302,
                    ExpectedResponseUri = new Uri(baseAddress, "/lorem")
                }.ToTestCase("Absolute redirect rule", "A redirect with an absolute url should match on a url with that exact url"),

                // a redirect with a regex should match on any url of which the path matches the regex
                new RedirectTestCase
                {
                    Redirects = new List<RedirectTestModelsRedirect>
                    {
                        new RedirectTestModelsRedirect
                        {
                            Culture = "nl",
                            Force = false,
                            Id = 0,
                            Notes = "lorem",
                            PassThroughQueryString = false,
                            SourceRegex = @"^[0-9]{6}\?lorem=ipsum$",
                            SourceUrl = null,
                            TargetNodeName = "lorem",
                            TargetRootNodeName = "root",
                            TargetStatusCode = 302,
                            TargetUrl = null
                        }
                    },
                    RequestUrl = new Uri(baseAddress, "/182579?lorem=ipsum"),
                    ExpectedResponseStatusCode = 302,
                    ExpectedResponseUri = new Uri(baseAddress, "/lorem")
                }.ToTestCase("Regex redirect rule", "A redirect with a regex should match on any url of which the path matches the regex"),

                // a redirect on a url that has an existing response should not redirect if it's not forced
                new RedirectTestCase
                {
                    Redirects = new List<RedirectTestModelsRedirect>
                    {
                        new RedirectTestModelsRedirect
                        {
                            Culture = "nl",
                            Force = false,
                            Id = 0,
                            Notes = "lorem",
                            PassThroughQueryString = true,
                            SourceRegex = null,
                            SourceUrl = new Uri(baseAddress, "/lorem/").AbsoluteUri,
                            TargetNodeName = "root",
                            TargetRootNodeName = "root",
                            TargetStatusCode = 302,
                            TargetUrl = null
                        }
                    },
                    RequestUrl = new Uri(baseAddress, "/lorem"),
                    ExpectedResponseStatusCode = 200,
                    ExpectedResponseUri = null
                }.ToTestCase("Existing response no force rule does not redirect", "A redirect on a url that has an existing response should not redirect if it's not forced"),

                // a redirect on a url that has an existing response should redirect if it's forced
                new RedirectTestCase
                {
                    Redirects = new List<RedirectTestModelsRedirect>
                    {
                        new RedirectTestModelsRedirect
                        {
                            Culture = "nl",
                            Force = true,
                            Id = 0,
                            Notes = "lorem",
                            PassThroughQueryString = true,
                            SourceRegex = null,
                            SourceUrl = new Uri(baseAddress, "/lorem/").AbsoluteUri,
                            TargetNodeName = "root",
                            TargetRootNodeName = "root",
                            TargetStatusCode = 302,
                            TargetUrl = null
                        }
                    },
                    RequestUrl = new Uri(baseAddress, "/lorem"),
                    ExpectedResponseStatusCode = 302,
                    ExpectedResponseUri = new Uri(baseAddress, "/")
                }.ToTestCase("Existing response force rule redirects", "A redirect on a url that has an existing response should redirect if it's forced"),

                // a redirect that is forced should have priority over a more recently added redirect that is not forced
                new RedirectTestCase
                {
                    Redirects = new List<RedirectTestModelsRedirect>
                    {
                        new RedirectTestModelsRedirect
                        {
                            Culture = "nl",
                            Force = false,
                            Id = 0,
                            Notes = "lorem",
                            PassThroughQueryString = true,
                            SourceRegex = null,
                            SourceUrl = new Uri(baseAddress, "/lorem/").AbsoluteUri,
                            TargetNodeName = "root",
                            TargetRootNodeName = "root",
                            TargetStatusCode = 301,
                            TargetUrl = null
                        },
                        new RedirectTestModelsRedirect
                        {
                            Culture = "nl",
                            Force = true,
                            Id = 0,
                            Notes = "lorem",
                            PassThroughQueryString = true,
                            SourceRegex = null,
                            SourceUrl = new Uri(baseAddress, "/lorem/").AbsoluteUri,
                            TargetNodeName = "ipsum",
                            TargetRootNodeName = "root",
                            TargetStatusCode = 302,
                            TargetUrl = null
                        }
                    },
                    RequestUrl = new Uri(baseAddress, "/lorem"),
                    ExpectedResponseStatusCode = 302,
                    ExpectedResponseUri = new Uri(baseAddress, "/ipsum")
                }.ToTestCase("Multiple redirect rules redirect on forced rule", "A redirect that is forced should have priority over a more recently added redirect that is not forced"),

                // when pass through query string is enabled, the query string should be passed along with the request
                new RedirectTestCase
                {
                    Redirects = new List<RedirectTestModelsRedirect>
                    {
                        new RedirectTestModelsRedirect
                        {
                            Culture = "nl",
                            Force = false,
                            Id = 0,
                            Notes = "lorem",
                            PassThroughQueryString = true,
                            SourceRegex = null,
                            SourceUrl = "/dolor",
                            TargetNodeName = null,
                            TargetRootNodeName = "root",
                            TargetStatusCode = 301,
                            TargetUrl = "/lorem"
                        }
                    },
                    RequestUrl = new Uri(baseAddress, "/dolor?sit=amet"),
                    ExpectedResponseStatusCode = 301,
                    ExpectedResponseUri = new Uri(baseAddress, "/lorem?sit=amet")
                }.ToTestCase("Pass through query string maintains query string", "A redirect with pass through query string enabled should maintain the query string in the redirect"),

                // when pass through query string is enabled, the query string should be passed along with the request
                new RedirectTestCase
                {
                    Redirects = new List<RedirectTestModelsRedirect>
                    {
                        new RedirectTestModelsRedirect
                        {
                            Culture = "nl",
                            Force = false,
                            Id = 0,
                            Notes = "lorem",
                            PassThroughQueryString = false,
                            SourceRegex = null,
                            SourceUrl = "/dolor",
                            TargetNodeName = null,
                            TargetRootNodeName = "root",
                            TargetStatusCode = 301,
                            TargetUrl = "/lorem"
                        }
                    },
                    RequestUrl = new Uri(baseAddress, "/dolor?sit=amet"),
                    ExpectedResponseStatusCode = 301,
                    ExpectedResponseUri = new Uri(baseAddress, "/lorem")
                }.ToTestCase("Pass through query string disabled removes query string", "A redirect without pass through query string enabled should remove the query string in the redirect"),

                // when redirect has a query string, it should not match if the incoming redirect does not have this exact query string
                new RedirectTestCase
                {
                    Redirects = new List<RedirectTestModelsRedirect>
                    {
                        new RedirectTestModelsRedirect
                        {
                            Culture = "nl",
                            Force = false,
                            Id = 0,
                            Notes = "lorem",
                            PassThroughQueryString = false,
                            SourceRegex = null,
                            SourceUrl = "/dolor?sit=amet",
                            TargetNodeName = null,
                            TargetRootNodeName = "root",
                            TargetStatusCode = 301,
                            TargetUrl = "/lorem"
                        }
                    },
                    RequestUrl = new Uri(baseAddress, "/dolor?sit=amet&conseptitur=adipiscin"),
                    ExpectedResponseStatusCode = 404,
                    ExpectedResponseUri = null
                }.ToTestCase("Redirect with query string does not redirect url with different query string", "When redirect has a query string, it should not match if the incoming redirect does not have this exact query string"),

                // when redirect has a query string, it should match if the incoming redirect has this exact query string
                new RedirectTestCase
                {
                    Redirects = new List<RedirectTestModelsRedirect>
                    {
                        new RedirectTestModelsRedirect
                        {
                            Culture = "nl",
                            Force = false,
                            Id = 0,
                            Notes = "lorem",
                            PassThroughQueryString = false,
                            SourceRegex = null,
                            SourceUrl = "/dolor?sit=amet",
                            TargetNodeName = null,
                            TargetRootNodeName = "root",
                            TargetStatusCode = 301,
                            TargetUrl = "/lorem"
                        }
                    },
                    RequestUrl = new Uri(baseAddress, "/dolor?sit=amet"),
                    ExpectedResponseStatusCode = 301,
                    ExpectedResponseUri = new Uri(baseAddress, "/lorem")
                }.ToTestCase("Relative redirect with query string redirects url with same query string", "When redirect has a query string, it should match if the incoming redirect has this exact query string"),

                // when redirect has a query string, it should match if the incoming redirect has this exact query string
                new RedirectTestCase
                {
                    Redirects = new List<RedirectTestModelsRedirect>
                    {
                        new RedirectTestModelsRedirect
                        {
                            Culture = "nl",
                            Force = false,
                            Id = 0,
                            Notes = "lorem",
                            PassThroughQueryString = false,
                            SourceRegex = null,
                            SourceUrl = new Uri(baseAddress, "/dolor/?sit=amet").AbsoluteUri,
                            TargetNodeName = null,
                            TargetRootNodeName = "root",
                            TargetStatusCode = 301,
                            TargetUrl = "/lorem"
                        }
                    },
                    RequestUrl = new Uri(baseAddress, "/dolor?sit=amet"),
                    ExpectedResponseStatusCode = 301,
                    ExpectedResponseUri = new Uri(baseAddress, "/lorem")
                }.ToTestCase("Absolute redirect with query string redirects url with same query string", "When redirect has a query string, it should match if the incoming redirect has this exact query string"),

                new RedirectTestCase
                {
                    Redirects = new List<RedirectTestModelsRedirect>
                    {
                        new RedirectTestModelsRedirect
                        {
                            Culture = "nl",
                            TargetRootNodeName = "root",
                            SourceUrl = new Uri(baseAddress, "/dolor/").AbsoluteUri,
                            TargetStatusCode = 410,
                        }
                    },
                    RequestUrl = new Uri(baseAddress, "/dolor"),
                    ExpectedResponseStatusCode = 410,
                    ExpectedResponseUri = null
                }.ToTestCase("No longer exists entry rewrites 404 response to 410", "When an entry in the url tracker is present for a 410 and the original response is a 404, then the url tracker should change the response to 410"),

                new RedirectTestCase
                {
                    Redirects = new List<RedirectTestModelsRedirect>
                    {
                        new RedirectTestModelsRedirect
                        {
                            Culture = "nl",
                            TargetRootNodeName = "root",
                            SourceUrl = new Uri(baseAddress, "/lorem/").AbsoluteUri,
                            TargetStatusCode = 410
                        }
                    },
                    RequestUrl = new Uri(baseAddress, "/lorem"),
                    ExpectedResponseStatusCode = 200,
                    ExpectedResponseUri = null
                }.ToTestCase("No longer exists entry does not touch 200 responses", "When an entry in the url tracker is present for a 410 and the original response is a 200, then the url tracker does nothing"),

                new RedirectTestCase()
                {
                    Redirects = new List<RedirectTestModelsRedirect>
                    {
                        new RedirectTestModelsRedirect
                        {
                            Culture = null,
                            SourceUrl = "lorem/ipsum",
                            TargetRootNodeName= "root",
                            TargetNodeName = "lorem",
                            TargetStatusCode = 302
                        }
                    },
                    RequestUrl = new Uri(baseAddress, "/lorem/ipsum"),
                    ExpectedResponseStatusCode = 302,
                    ExpectedResponseUri = new Uri(baseAddress, "/lorem")
                }.ToTestCase("Redirect with relative redirect without leading slash redirects correctly", "Users might enter relative urls without leading slash. Redirects should still work properly for them")
            };
        }
    }
}
