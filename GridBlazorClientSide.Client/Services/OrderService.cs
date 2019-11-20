using GridBlazorClientSide.Shared.Models;
using GridShared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GridBlazorClientSide.Client.Services
{
    public class OrderService : IOrderService
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
            await _httpClient.SendJsonAsync(HttpMethod.Delete, _baseUri + $"api/Order/{orderId}", null);
        }

        public IList<Tuple<string, string>> GetAllCustomers()
        {
            try
            {
                var customers = _httpClient.GetJsonAsync<Tuple<string, string>[]>(_baseUri + $"api/SampleData/GetAllCustomers").Result;
                return customers.ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<Tuple<string, string>>();
            }
        }

        public IList<Tuple<string, string>> GetAllEmployees()
        {
            try
            {
                var employees = _httpClient.GetJsonAsync<Tuple<string, string>[]>(_baseUri + $"api/SampleData/GetAllEmployees").Result;
                return employees.ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<Tuple<string, string>>();
            }
        }

        public IList<Tuple<string, string>> GetAllShippers()
        {
            try
            {
                var shippers = _httpClient.GetJsonAsync<Tuple<string, string>[]>(_baseUri + $"api/SampleData/GetAllShippers").Result;
                return shippers.ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<Tuple<string, string>>();
            }
        }
    }

    public interface IOrderService : ICrudDataService<Order>
    {
        IList<Tuple<string, string>> GetAllCustomers();
        IList<Tuple<string, string>> GetAllEmployees();
        IList<Tuple<string, string>> GetAllShippers();
    }
}
