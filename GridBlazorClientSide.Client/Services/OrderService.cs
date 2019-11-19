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

        public IList<Tuple<string, string>> GetAllCustomers()
        {
            try
            {
                var customers = _http.GetJsonAsync<Tuple<string, string>[]>($"api/SampleData/GetAllCustomers").Result;
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
                return _http.GetJsonAsync<Tuple<string, string>[]>($"api/SampleData/GetAllEmployees").Result;
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
                return _http.GetJsonAsync<Tuple<string, string>[]>($"api/SampleData/GetAllShippers").Result;
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
