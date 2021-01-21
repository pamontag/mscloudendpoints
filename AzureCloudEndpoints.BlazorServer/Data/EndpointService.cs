using AzureCloudEndpoints.BlazorServer.Model;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;
using System.Runtime.CompilerServices;

namespace AzureCloudEndpoints.BlazorServer.Data
{
    
    public class EndpointService
    {
        public static HttpClient client = new HttpClient();
        private readonly IMemoryCache _memoryCache;
        private static readonly string OFFICE365_KEY = "Office365";
        private static readonly string AZURE_KEY = "Azure";

        public EndpointService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public async Task<Office365Endpoint[]> GetOffice365EndpointsAsync()
        {
            var guid = Guid.NewGuid();
            var office365data = GetCacheData(OFFICE365_KEY);
            if(office365data == null)
            {
                var response = await client.GetAsync($"https://endpoints.office.com/endpoints/worldwide?clientrequestid={guid.ToString()}");
                office365data = SetCacheData(OFFICE365_KEY, await response.Content.ReadAsStringAsync());
            }
            
            return JsonConvert.DeserializeObject<Office365Endpoint[]>(office365data);
        }

        public async Task<AzureEndpointInfo> GetAzureEndpointsAsync()
        {
            var date = DateTime.Now;
            var retry = 0;
            HttpResponseMessage response = null;
            var azuredata = GetCacheData(AZURE_KEY);
            if (azuredata == null)
            {
                do
                {
                    var dateString = date.ToString("yyyyMMdd");
                    response = await client.GetAsync($"https://download.microsoft.com/download/7/1/D/71D86715-5596-4529-9B13-DA13A5DE5B63/ServiceTags_Public_{dateString}.json");
                    date = date.AddDays(-1);
                    retry++;
                } while (response.StatusCode == System.Net.HttpStatusCode.NotFound && retry < 50);
                if (response.IsSuccessStatusCode)
                    azuredata = SetCacheData(AZURE_KEY, await response.Content.ReadAsStringAsync());
                else
                    return null;

            }
            return JsonConvert.DeserializeObject<AzureEndpointInfo>(azuredata);
        }

        private string GetCacheData(string key)
        { 
            var encodedCache = _memoryCache.Get(key);
            return encodedCache != null ? encodedCache.ToString() : null; 
        }

        private string SetCacheData(string key, string value)
        {
            var options = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromHours(6))
                    .SetAbsoluteExpiration(DateTime.Now.AddHours(6));
            _memoryCache.Set(key, value, options);
            return _memoryCache.Get(key).ToString();
        }
    }
}
