using GridBlazorClientSide.Shared.Models;
using GridMvc.Server;
using GridShared;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GridBlazorClientSide.Client.Services
{
    public class OrderGridInMemoryService : IOrderGridInMemoryService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUri;

        private IEnumerable<Order> _orders;

        public OrderGridInMemoryService(HttpClient httpClient, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _baseUri = navigationManager.BaseUri;
        }

        public async Task<ItemsDTO<Order>> GetOrdersGridRowsInMemory(Action<IGridColumnCollection<Order>> columns,
            QueryDictionary<StringValues> query)
        {
            var server = new GridServer<Order>(await GetAll(), new QueryCollection(query), true, "ordersGrid", columns)
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
                _orders = await _httpClient.GetFromJsonAsync<IEnumerable<Order>>(_baseUri + $"api/Order/");
            return _orders;
        }
    }

    public interface IOrderGridInMemoryService
    {
        Task<ItemsDTO<Order>> GetOrdersGridRowsInMemory(Action<IGridColumnCollection<Order>> columns,
            QueryDictionary<StringValues> query);
    }
}
