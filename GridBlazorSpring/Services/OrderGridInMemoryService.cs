using GridBlazorSpring.Models;
using GridCore.Server;
using GridShared;
using GridShared.Utility;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GridBlazorSpring.Services
{
    public class OrderGridInMemoryService : IOrderGridInMemoryService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUri;

        private IEnumerable<Order> _orders;

        public OrderGridInMemoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _baseUri = "http://localhost:8080/";
        }

        public async Task<ItemsDTO<Order>> GetOrdersGridRowsInMemory(Action<IGridColumnCollection<Order>> columns,
            QueryDictionary<StringValues> query)
        {
            var server = new GridCoreServer<Order>(await GetAll(), query, true, "ordersGrid", columns)
                        .Sortable()
                        .WithPaging(10)
                        .Filterable()
                        .WithMultipleFilters()
                        .Groupable(true)
                        .Searchable(true, false, false);

            // return items to displays
            var items = server.ItemsToDisplay;

            return items;
        }

        private async Task<IEnumerable<Order>> GetAll()
        {
            if (_orders == null)
                _orders = await _httpClient.GetFromJsonAsync<IEnumerable<Order>>(_baseUri + $"api/Order");
            return _orders;
        }
    }

    public interface IOrderGridInMemoryService
    {
        Task<ItemsDTO<Order>> GetOrdersGridRowsInMemory(Action<IGridColumnCollection<Order>> columns,
            QueryDictionary<StringValues> query);
    }
}
