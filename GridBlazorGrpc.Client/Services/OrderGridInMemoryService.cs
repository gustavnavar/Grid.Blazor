using GridBlazorGrpc.Shared.Models;
using GridBlazorGrpc.Shared.Services;
using GridCore.Server;
using GridShared;
using GridShared.Utility;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Primitives;
using ProtoBuf.Grpc.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace GridBlazorGrpc.Client.Services
{
    public class OrderGridInMemoryService : IOrderGridInMemoryService
    {
        private readonly string _baseUri;

        public OrderGridInMemoryService(NavigationManager navigationManager)
        {
            _baseUri = navigationManager.BaseUri;
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
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions()
            {
                HttpClient = new HttpClient(handler),
                MaxSendMessageSize = 214748364,
                MaxReceiveMessageSize = 2147483647
            }))
            {
                var service = channel.CreateGrpcService<IOrderService>();
                return await service.GetAll();
            }
        }
    }

    public interface IOrderGridInMemoryService
    {
        Task<ItemsDTO<Order>> GetOrdersGridRowsInMemory(Action<IGridColumnCollection<Order>> columns,
            QueryDictionary<StringValues> query);
    }
}
