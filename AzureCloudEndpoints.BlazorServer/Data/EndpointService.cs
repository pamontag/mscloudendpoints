using AzureCloudEndpoints.BlazorServer.Model;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AzureCloudEndpoints.BlazorServer.Data
{
    public class EndpointService
    {
        public static HttpClient client = new HttpClient();
        public async Task<Office365Endpoint[]> GetOffice365EndpointsAsync()
        {
            var guid = Guid.NewGuid();
            var response = await client.GetAsync($"https://endpoints.office.com/endpoints/worldwide?clientrequestid={guid.ToString()}");
            return JsonConvert.DeserializeObject<Office365Endpoint[]>(await response.Content.ReadAsStringAsync());
        }

        public async Task<AzureEndpointInfo> GetAzureEndpointsAsync()
        {
            var date = DateTime.Now;
            var retry = 0;
            HttpResponseMessage response = null;
            do
            {
                var dateString = date.ToString("yyyyMMdd");
                response = await client.GetAsync($"https://download.microsoft.com/download/7/1/D/71D86715-5596-4529-9B13-DA13A5DE5B63/ServiceTags_Public_{dateString}.json");
                date = date.AddDays(-1);
                retry++;
            } while (response.StatusCode == System.Net.HttpStatusCode.NotFound && retry < 50);
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<AzureEndpointInfo>(await response.Content.ReadAsStringAsync());
            else
                return null;
        }
    }
}
