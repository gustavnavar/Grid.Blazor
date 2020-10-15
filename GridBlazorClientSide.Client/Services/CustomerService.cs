using GridBlazorClientSide.Shared.Models;
using GridShared;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GridBlazorClientSide.Client.Services
{
    public class CustomerService : ICrudDataService<Customer>
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUri;

        public CustomerService(HttpClient httpClient, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _baseUri = navigationManager.BaseUri;
        }

        public async Task<Customer> Get(params object[] keys)
        {
            return await _httpClient.GetFromJsonAsync<Customer>(_baseUri + $"api/Customer/{keys[0].ToString()}");
        }

        public async Task Insert(Customer item)
        {
            var response = await _httpClient.PostAsJsonAsync(_baseUri + $"api/Customer", item);
            if (!response.IsSuccessStatusCode)
            {
                throw new GridException("CUSSRV-01", "Error creating the customer");
            }
        }

        public async Task Update(Customer item)
        {
            var response = await _httpClient.PutAsJsonAsync(_baseUri + $"api/Customer/{item.CustomerID}", item);
            if (!response.IsSuccessStatusCode)
            {
                throw new GridException("CUSSRV-02", "Error updating the customer");
            }
        }

        public async Task Delete(params object[] keys)
        {
            var response = await _httpClient.DeleteAsync(_baseUri + $"api/Customer/{keys[0].ToString()}");
            if (!response.IsSuccessStatusCode)
            {
                throw new GridException("CUSSRV-03", "Error deleting the customer");
            }
        }
    }
}
