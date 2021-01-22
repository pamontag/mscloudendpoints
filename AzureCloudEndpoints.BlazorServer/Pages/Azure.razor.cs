using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureCloudEndpoints.BlazorServer.Model;
using AzureCloudEndpoints.BlazorServer.Data;

namespace AzureCloudEndpoints.BlazorServer.Pages
{
    public partial class Azure : ComponentBase
    {
        [Inject]
        public EndpointService endpointService { get; set; }
        [Inject]
        public NavigationManager navigationManager { get; set; }

        public AzureEndpoint[] Endpoints { get; set; }
        public AzureEndpoint[] SelectedEndpoints { get; set; }
        public string[] Services { get; set; }
        public string SelectedService { get; set; }
        public string[] Regions { get; set; }
        public string SelectedRegion { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Endpoints = (await endpointService.GetAzureEndpointsAsync()).values.ToArray();
            Endpoints.Where(e => e.properties.region == "").ToList().ForEach(e => e.properties.region = "global");
            Endpoints.Where(e => e.properties.systemService == "").ToList().ForEach(e => e.properties.systemService = "AzureCloud");
            Services = Endpoints.Select(e => e.properties.systemService).Distinct().OrderBy(e => e).ToArray();
            SelectedService = Services.Single(r => r == "AzureCloud");
            Regions = Endpoints.Select(e => e.properties.region).Distinct().OrderBy(e => e).ToArray();
            SelectedRegion = Regions.Single(r => r == "westeurope");
            SelectedEndpoints = Endpoints.Where(e => e.properties.systemService == SelectedService && e.properties.region == SelectedRegion).ToArray();
        }

        public void OnChangeService(string service)
        {
            SelectedService = service;
            SelectedEndpoints = Endpoints.Where(e => e.properties.systemService == SelectedService && e.properties.region == SelectedRegion).ToArray();
            if (SelectedEndpoints.Count() == 0)
            {
                OnChangeRegion("global");
            }
        }

        public void OnChangeRegion(string region)
        {
            SelectedRegion = region;
            SelectedEndpoints = Endpoints.Where(e => e.properties.systemService == SelectedService && e.properties.region == SelectedRegion).ToArray();
        }

        void OnClickDownloadButton()
        {
            navigationManager.NavigateTo($"api/download/{SelectedService}/{SelectedRegion}", true);

        }
    }
}
