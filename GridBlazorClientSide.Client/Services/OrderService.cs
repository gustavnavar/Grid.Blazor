using GridBlazorClientSide.Shared.Models;
using GridShared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace GridBlazorClientSide.Client.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _http;

        public OrderService(HttpClient http)
        {
            _http = http;
        }

        public async Task<Order> Get(params object[] keys)
        {
            int orderId;
            int.TryParse(keys[0].ToString(), out orderId);
            return await _http.GetJsonAsync<Order>($"api/Order/{orderId}");
        }

        public async Task Insert(Order item)
        {
            await _http.PostJsonAsync($"api/Order/{item.OrderID}", item);
        }

        public async Task Update(Order item)
        {
            await _http.PutJsonAsync($"api/Order/{item.OrderID}", item);
        }

        public async Task Delete(params object[] keys)
        {
            int orderId;
            int.TryParse(keys[0].ToString(), out orderId);
            await _http.SendJsonAsync(HttpMethod.Delete, $"api/Order/{orderId}", null);
        }

        public IEnumerable<Tuple<string, string>> GetAllCustomers()
        {
            return _http.GetJsonAsync<IEnumerable<Tuple<string, string>>>($"api/SampleData/GetAllCustomers").Result;
        }

        public IEnumerable<Tuple<string, string>> GetAllEmployees()
        {
            return _http.GetJsonAsync<IEnumerable<Tuple<string, string>>>($"api/SampleData/GetAllEmployees").Result;
        }

        public IEnumerable<Tuple<string, string>> GetAllShippers()
        {
            return _http.GetJsonAsync<IEnumerable<Tuple<string, string>>>($"api/SampleData/GetAllShippers").Result;
        }
    }

    public interface IOrderService : ICrudDataService<Order>
    {
        IEnumerable<Tuple<string, string>> GetAllCustomers();
        IEnumerable<Tuple<string, string>> GetAllEmployees();
        IEnumerable<Tuple<string, string>> GetAllShippers();
    }
}
