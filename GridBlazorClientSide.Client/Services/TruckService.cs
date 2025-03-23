using GridBlazorClientSide.Shared.Models;
using GridShared;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GridBlazorClientSide.Client.Services
{
    public class TruckService : ICrudDataService<Truck>
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUri;

        public TruckService(HttpClient httpClient, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _baseUri = navigationManager.BaseUri;
        }

        public async Task<Truck> Get(params object[] keys)
        {
            int truckId;
            int.TryParse(keys[0].ToString(), out truckId);
            return await _httpClient.GetFromJsonAsync<Truck>(_baseUri + $"api/Truck/{truckId}");
        }

        public async Task Insert(Truck item)
        {
            await Task.CompletedTask;
        }

        public async Task Update(Truck item)
        {
            await Task.CompletedTask;
        }

        public async Task Delete(params object[] keys)
        {
            await Task.CompletedTask;
        }
    }
}
