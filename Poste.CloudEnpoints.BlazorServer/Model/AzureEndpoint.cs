using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureCloudEndpoints.BlazorServer.Model
{ 

    public class AzureEndpontProperties
    {
        public int changeNumber { get; set; }
        public string region { get; set; }
        public int regionId { get; set; }
        public string platform { get; set; }
        public string systemService { get; set; }
        public List<string> addressPrefixes { get; set; }
        public List<string> networkFeatures { get; set; }
    }

    public class AzureEndpoint
    {
        public string name { get; set; }
        public string id { get; set; }
        public AzureEndpontProperties properties { get; set; }
    }

    public class AzureEndpointInfo
    {
        public int changeNumber { get; set; }
        public string cloud { get; set; }
        public List<AzureEndpoint> values { get; set; }
    }

}
