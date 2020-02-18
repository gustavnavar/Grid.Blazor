using GridBlazorClientSide.Shared.Models;
using GridShared;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
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
            return await _httpClient.GetJsonAsync<OrderDetail>(_baseUri + $"api/OrderDetail/{orderId}/{productId}");
        }

        public async Task Insert(OrderDetail item)
        {
            await _httpClient.PostJsonAsync(_baseUri + $"api/OrderDetail", item);
        }

        public async Task Update(OrderDetail item)
        {
            await _httpClient.PutJsonAsync(_baseUri + $"api/OrderDetail/{item.OrderID}/{item.ProductID}", item);
        }

        public async Task Delete(params object[] keys)
        {
            int orderId;
            int.TryParse(keys[0].ToString(), out orderId);
            int productId;
            int.TryParse(keys[1].ToString(), out productId);
            await _httpClient.SendJsonAsync(HttpMethod.Delete, _baseUri + $"api/OrderDetail/{orderId}/{productId}", null);
        }
    }
}
