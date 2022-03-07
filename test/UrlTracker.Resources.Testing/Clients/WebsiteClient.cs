using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UrlTracker.Resources.Testing.Clients.Models;

namespace UrlTracker.Resources.Testing.Clients
{
    public class WebsiteClient : HttpClient
    {
        private WebsiteClient(string baseUrl, HttpClientHandler handler)
            : base(handler)
        {
            BaseAddress = new Uri(baseUrl);
        }

        public static WebsiteClient Create(string baseUrl)
        {
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false
            };

            return new WebsiteClient(baseUrl, handler);
        }

        public Task<HttpResponseMessage> ResetAsync()
        {
            return PostAsync("/api/preset/reset", null);
        }

        public Task<HttpResponseMessage> SeedRedirectAsync(SeedRedirectRequest request)
        {
            return PostAsync("/api/preset/seedredirect", new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
        }

        public async Task<ContentTreeViewModelCollection> GetTreeAsync()
        {
            var result = await GetAsync("/api/preset/tree").ConfigureAwait(false);
            result.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<ContentTreeViewModelCollection>(await result.Content.ReadAsStringAsync());
        }
    }
}
