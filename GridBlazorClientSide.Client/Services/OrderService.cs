using GridBlazorClientSide.Shared.Models;
using GridShared;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Threading.Tasks;

namespace GridBlazorClientSide.Client.Services
{
    public class OrderService : ICrudDataService<Order>
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUri;

        public OrderService(HttpClient httpClient, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _baseUri = navigationManager.BaseUri;
        }

        public async Task<Order> Get(params object[] keys)
        {
            int orderId;
            int.TryParse(keys[0].ToString(), out orderId);
            return await _httpClient.GetJsonAsync<Order>(_baseUri + $"api/Order/{orderId}");
        }

        public async Task Insert(Order item)
        {
            await _httpClient.PostJsonAsync(_baseUri + $"api/Order", item);
        }

        public async Task Update(Order item)
        {
            await _httpClient.PutJsonAsync(_baseUri + $"api/Order/{item.OrderID}", item);
        }

        public async Task Delete(params object[] keys)
        {
            int orderId;
            int.TryParse(keys[0].ToString(), out orderId);
            var response = await _httpClient.DeleteAsync(_baseUri + $"api/Order/{orderId}");
            if (!response.IsSuccessStatusCode)
            {
                throw new GridException("ORDSRV-03", "Error deleting the order");
            }
        }
    }
}
