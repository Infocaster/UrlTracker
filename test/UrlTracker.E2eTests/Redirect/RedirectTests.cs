using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using UrlTracker.Resources.Testing.Clients;
using UrlTracker.Resources.Testing.Clients.Models;

namespace UrlTracker.E2eTests.Redirect
{
    public class RedirectTests
    {
        public ContentTreeViewModelCollection? ContentTree { get; set; }
        public Dictionary<string, ContentTreeViewModel>? ContentTable { get; set; }

        public static WebsiteClient WebsiteClient => ClientSetup.WebsiteClient;

        [SetUp]
        public void SetUp()
        {
            var result = WebsiteClient.ResetAsync().Result;
            result.EnsureSuccessStatusCode();
            ContentTree = WebsiteClient.GetTreeAsync().Result;
            ContentTable = MapNames(ContentTree ?? throw new Exception("ContentTree is null"));
        }

        private static Dictionary<string, ContentTreeViewModel> MapNames(ContentTreeViewModelCollection contentTree)
        {
            Dictionary<string, ContentTreeViewModel> result = new();

            static void DoMap(IEnumerable<ContentTreeViewModel> models, ref Dictionary<string, ContentTreeViewModel> output)
            {
                foreach (var model in models)
                {
                    output[model.Name!.ToLowerInvariant()] = model;
                    DoMap(model.Children!, ref output);
                }
            }

            DoMap(contentTree.RootContent!, ref result);
            return result;
        }

        protected async Task SeedRedirectsAsync(IEnumerable<RedirectTestModelsRedirect> redirects)
        {
            var result = await WebsiteClient.SeedRedirectAsync(new SeedRedirectRequest
            {
                Redirects = (from r in redirects
                             select new SeedRedirectRequestRedirect
                             {
                                 Culture = r.Culture,
                                 Force = r.Force,
                                 Id = r.Id,
                                 Notes = r.Notes,
                                 PassThroughQueryString = r.PassThroughQueryString,
                                 SourceRegex = r.SourceRegex,
                                 SourceUrl = r.SourceUrl,
                                 TargetNodeId = r.TargetNodeName is not null ? ContentTable![r.TargetNodeName].Id : (int?)null,
                                 TargetRootNodeId = r.TargetRootNodeName is not null ? ContentTable![r.TargetRootNodeName].Id : (int?)null,
                                 TargetStatusCode = r.TargetStatusCode,
                                 TargetUrl = r.TargetUrl
                             }).ToList()
            }).ConfigureAwait(false);
            result.EnsureSuccessStatusCode();
        }

        public static IEnumerable<TestCaseData> TestCaseSource()
        {
            return RedirectTestCasesSource.CreateValueSets(new Uri("http://urltracker.ic/"));
        }

        [TestCaseSource(nameof(TestCaseSource))]
        public async Task InsertBasicRedirect_NormalFlow_ServerReturnsRedirectResult(IEnumerable<RedirectTestModelsRedirect> redirects, Uri requestUrl, int expectedResponseCode, Uri expectedRedirectUrl)
        {
            // arrange
            await SeedRedirectsAsync(redirects).ConfigureAwait(false);

            // act
            var result = await WebsiteClient.GetAsync(requestUrl).ConfigureAwait(false);

            // assert
            Assert.Multiple(() =>
            {
                Assert.That((int)result.StatusCode, Is.EqualTo(expectedResponseCode));
                Assert.That(result.Headers.Location?.AbsoluteUri, Is.EqualTo(expectedRedirectUrl));
            });
        }
    }
}
