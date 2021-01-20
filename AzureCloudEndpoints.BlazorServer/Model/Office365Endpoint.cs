using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureCloudEndpoints.BlazorServer.Model
{
    public class Office365Endpoint
    {
        public int id { get; set; }
        public string serviceArea { get; set; }
        public string serviceAreaDisplayName { get; set; }
        public List<string> urls { get; set; }
        public List<string> ips { get; set; }
        public string tcpPorts { get; set; }
        public bool expressRoute { get; set; }
        public string category { get; set; }
        public bool required { get; set; }
        public string notes { get; set; }
        public string udpPorts { get; set; }
    }
}
