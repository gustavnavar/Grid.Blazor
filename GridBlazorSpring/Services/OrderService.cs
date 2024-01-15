using GridBlazorSpring.Models;
using GridShared;
using GridShared.Utility;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GridBlazorSpring.Services
{
    public class OrderService : ICrudDataService<Order>
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUri;

        public OrderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _baseUri = "http://localhost:8080/";
        }

        public async Task<Order> Get(params object[] keys)
        {
            int orderId;
            int.TryParse(keys[0].ToString(), out orderId);
            return await _httpClient.GetFromJsonAsync<Order>(_baseUri + $"api/Order/{orderId}");
        }

        public async Task Insert(Order item)
        {
            var response = await _httpClient.PostAsJsonAsync(_baseUri + $"api/Order", item);
            if (response.IsSuccessStatusCode)
            {
                item.OrderID = Convert.ToInt32(await response.Content.ReadAsStringAsync());
            }
            else
            {
                throw new GridException("ORDSRV-01", "Error creating the order");
            }
        }

        public async Task Update(Order item)
        {
            var response = await _httpClient.PutAsJsonAsync(_baseUri + $"api/Order/{item.OrderID}", item);
            if (!response.IsSuccessStatusCode)
            {
                throw new GridException("ORDSRV-02", "Error updating the order");
            }
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
