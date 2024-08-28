using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.Json;

namespace UrlTracker.GlobalBlocklist.Services
{
    public interface IRetrieveDenyListService
    {
        Task<DenyListResponse> GetGlobalSettings();
    }

    public class RetrieveDenyListService : IRetrieveDenyListService
    {
        private readonly HttpClient _httpClient;

        public RetrieveDenyListService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DenyListResponse> GetGlobalSettings()
        {
            var result = await _httpClient.GetAsync(Defaults.DenyList.DenyListUrl);
            result.EnsureSuccessStatusCode();
            var content = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DenyListResponse>(content);
        }
    }
}
