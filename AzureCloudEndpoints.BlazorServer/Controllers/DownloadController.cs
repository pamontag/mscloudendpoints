using AzureCloudEndpoints.BlazorServer.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AzureCloudEndpoints.BlazorServer.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class DownloadController: ControllerBase
    {
        private EndpointService _endpointService;
        public DownloadController(EndpointService endpointService)
        {
            this._endpointService = endpointService;
        }

        [HttpGet, Route("{serviceName}")]
        public async Task<ActionResult> Get(string serviceName)
        { 
            var endpoints = await _endpointService.GetOffice365EndpointsAsync(); 
            endpoints = endpoints.Where(e => e.serviceArea == serviceName).ToArray();
            var csv = "SERVICE,DESCRIPTION,URLs,IPs,TCP,UDP,REQUIRED,EXPRESSROUTE,CATEGORY,NOTES" + Environment.NewLine;             
            foreach (var endpoint in endpoints)
            {
                var tcp = endpoint.tcpPorts != null ? endpoint.tcpPorts.Replace(",", ";") : null;
                var udp = endpoint.udpPorts != null ? endpoint.udpPorts.Replace(",", ";") : null;
                var notes = endpoint.notes != null ? endpoint.notes.Replace(",", ";") : null;
                var urls = endpoint.urls != null ? String.Join(";", endpoint.urls) : null;
                var ips = endpoint.ips != null ? String.Join(";", endpoint.ips) : null;
                csv = csv + $"{endpoint.serviceArea},{endpoint.serviceAreaDisplayName},{urls},{ips},{tcp},{udp},{endpoint.required},{endpoint.expressRoute},{endpoint.category},{notes}" + Environment.NewLine;
            }

            var buffer = Encoding.UTF8.GetBytes(csv);
            var stream = new MemoryStream(buffer); 
            var result = new FileStreamResult(stream, "text/plain");
            result.FileDownloadName = $"office365ips_{serviceName}.csv";
            return result;
        }
        [HttpGet, Route("{serviceName}/{regionName}")]
        public async Task<ActionResult> Get(string serviceName, string regionName)
        { 
            var endpointsInfo = await _endpointService.GetAzureEndpointsAsync();
            endpointsInfo.values.Where(e => e.properties.region == "").ToList().ForEach(e => e.properties.region = "global");
            endpointsInfo.values.Where(e => e.properties.systemService == "").ToList().ForEach(e => e.properties.systemService = "AzureCloud");
            var endpoints = endpointsInfo.values.Where(e => e.properties.systemService == serviceName && e.properties.region == regionName);
            var csv = "SERVICE,DESCRIPTION,REGION,PLATFORM,ADDRESSPREFIXES,NETWORKFEATURES" + Environment.NewLine;
            foreach (var endpoint in endpoints)
            { 
                var ips = endpoint.properties.addressPrefixes != null ? String.Join(";", endpoint.properties.addressPrefixes) : null;
                var nfet = endpoint.properties.networkFeatures != null ? String.Join(";", endpoint.properties.networkFeatures) : null;
                csv = csv + $"{endpoint.properties.systemService},{endpoint.name},{endpoint.properties.region},{endpoint.properties.platform},{ips},{nfet}" + Environment.NewLine;
            }

            var buffer = Encoding.UTF8.GetBytes(csv);
            var stream = new MemoryStream(buffer);
            var result = new FileStreamResult(stream, "text/plain");
            result.FileDownloadName = $"cloudips_{serviceName}_{regionName}.csv";
            return result;
        }
    }
}
