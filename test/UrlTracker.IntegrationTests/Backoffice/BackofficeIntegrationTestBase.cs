using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UrlTracker.IntegrationTests.Backoffice
{
    public abstract class BackofficeIntegrationTestBase : IntegrationTestBase
    {
        protected async Task<T?> DeserializeResponseAsync<T>(HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(body.Substring(6));
        }
    }
}
