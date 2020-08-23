using GridBlazorClientSide.Shared.Models;
using GridShared;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GridBlazorClientSide.Client.Services
{
    public class OrderDetailService : ICrudDataService<OrderDetail>
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUri;

        public OrderDetailService(HttpClient httpClient, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _baseUri = navigationManager.BaseUri;
        }

        public async Task<OrderDetail> Get(params object[] keys)
        {
            int orderId;
            int.TryParse(keys[0].ToString(), out orderId);
            int productId;
            int.TryParse(keys[1].ToString(), out productId);
            return await _httpClient.GetFromJsonAsync<OrderDetail>(_baseUri + $"api/OrderDetail/{orderId}/{productId}");
        }

        public async Task Insert(OrderDetail item)
        {
            var response = await _httpClient.PostAsJsonAsync(_baseUri + $"api/OrderDetail", item);
            if (!response.IsSuccessStatusCode)
            {
                throw new GridException("DETSRV-01", "Error creating the order detail");
            }
        }

        public async Task Update(OrderDetail item)
        {
            var response = await _httpClient.PutAsJsonAsync(_baseUri + $"api/OrderDetail/{item.OrderID}/{item.ProductID}", item);
            if (!response.IsSuccessStatusCode)
            {
                throw new GridException("DETSRV-02", "Error updating the order detail");
            }
        }

        public async Task Delete(params object[] keys)
        {
            int orderId;
            int.TryParse(keys[0].ToString(), out orderId);
            int productId;
            int.TryParse(keys[1].ToString(), out productId);
            var response = await _httpClient.DeleteAsync(_baseUri + $"api/OrderDetail/{orderId}/{productId}");
            if (!response.IsSuccessStatusCode)
            {
                throw new GridException("DETSRV-03", "Error deleting the order detail");
            }
        }
    }
}
