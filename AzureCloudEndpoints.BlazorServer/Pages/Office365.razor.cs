using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureCloudEndpoints.BlazorServer.Data;
using Microsoft.AspNetCore.Components;
using AzureCloudEndpoints.BlazorServer.Model;

namespace AzureCloudEndpoints.BlazorServer.Pages
{
    public partial class Office365 : ComponentBase
    {
        [Inject]
        public EndpointService endpointService { get; set; }
        [Inject]
        public NavigationManager navigationManager { get; set; }

        public Office365Endpoint[] Endpoints { get; set; }
        public Office365Endpoint[] SelectedEndpoints { get; set; }
        public string[] Services { get; set; }
        public string SelectedService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            SelectedEndpoints = Endpoints = await endpointService.GetOffice365EndpointsAsync();
            Services = Endpoints.Select(e => e.serviceArea).Distinct().ToArray();
            SelectedService = Services.First();
            SelectedEndpoints = Endpoints.Where(e => e.serviceArea == SelectedService).ToArray();
        }

        void OnChangeService(string service)
        {
            SelectedService = service;
            SelectedEndpoints = Endpoints.Where(e => e.serviceArea == SelectedService).ToArray();
        }

        void OnClickDownloadButton()
        {
            navigationManager.NavigateTo($"api/download/{SelectedService}", true);

        }
    }
}
