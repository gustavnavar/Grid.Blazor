using GridBlazorGrpc.Shared.Models;
using GridBlazorGrpc.Shared.Services;
using GridShared;
using GridShared.Utility;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components;
using ProtoBuf.Grpc.Client;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace GridBlazorGrpc.Client.Services
{
    public class GridClientService : IGridClientService
    {
        private readonly string _baseUri;

        public GridClientService(NavigationManager navigationManager)
        {
            _baseUri = navigationManager.BaseUri;
        }

        public async Task<Response> Add1ToFreight(int id)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IGridService>();
                return await service.Add1ToFreight(new Request(id));
            }
        }

        public async Task<IEnumerable<SelectItem>> GetAllContacts()
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IGridService>();
                return await service.GetAllContacts();
            }
        }

        public async Task<IEnumerable<SelectItem>> GetAllCustomers()
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IGridService>();
                return await service.GetAllCustomers();
            }
        }

        public async Task<IEnumerable<SelectItem>> GetAllCustomers2()
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IGridService>();
                return await service.GetAllCustomers2();
            }
        }

        public async Task<IEnumerable<SelectItem>> GetAllEmployees()
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IGridService>();
                return await service.GetAllEmployees();
            }
        }

        public async Task<IEnumerable<SelectItem>> GetAllProducts()
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IGridService>();
                return await service.GetAllProducts();
            }
        }

        public async Task<IEnumerable<SelectItem>> GetAllShippers()
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IGridService>();
                return await service.GetAllShippers();
            }
        }

        public async Task<ItemsDTO<Customer>> GetCustomersGrid(QueryDictionary<string> query)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IGridService>();
                return await service.GetCustomersGrid(query);
            }
        }

        public async Task<IEnumerable<string>> GetCustomersNames(QueryDictionary<string> query)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IGridService>();
                return await service.GetCustomersNames(query);
            }
        }

        public async Task<ItemsDTO<Employee>> GetEmployeesGrid(QueryDictionary<string> query)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IGridService>();
                return await service.GetEmployeesGrid(query);
            }
        }

        public async Task<Order> GetMaxFreight(QueryDictionary<string> query, string clientName)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IGridService>();
                return await service.GetMaxFreight(new Request(clientName, query));
            }
        }

        public async Task<Order> GetMinFreight(QueryDictionary<string> query, string clientName)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IGridService>();
                return await service.GetMinFreight(new Request(clientName, query));
            }
        }

        public async Task<ItemsDTO<Order>> GetOrderColumnsWithErrors(QueryDictionary<string> query)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IGridService>();
                return await service.GetOrderColumnsWithErrors(query);
            }
        }

        public async Task<ItemsDTO<Order>> GetOrdersGridAllFeatures(QueryDictionary<string> query)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IGridService>();
                return await service.GetOrdersGridAllFeatures(query);
            }
        }

        public async Task<ItemsDTO<Order>> GetOrdersGridExtSorting(QueryDictionary<string> query)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IGridService>();
                return await service.GetOrdersGridExtSorting(query);
            }
        }

        public async Task<ItemsDTO<Order>> GetOrdersGridGroupable(QueryDictionary<string> query)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IGridService>();
                return await service.GetOrdersGridGroupable(query);
            }
        }

        public async Task<ItemsDTO<Order>> GetOrdersGridordersAutoGenerateColumns(QueryDictionary<string> query)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IGridService>();
                return await service.GetOrdersGridordersAutoGenerateColumns(query);
            }
        }

        public async Task<ItemsDTO<Order>> GetOrdersGridRows(QueryDictionary<string> query)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IGridService>();
                return await service.GetOrdersGrid(query);
            }
        }

        public async Task<ItemsDTO<Order>> GetOrdersGridSearchable(QueryDictionary<string> query)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IGridService>();
                return await service.GetOrdersGridSearchable(query);
            }
        }

        public async Task<ItemsDTO<Order>> GetOrdersGridWithCount(QueryDictionary<string> query)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IGridService>();
                return await service.GetOrdersGridWithCount(query);
            }
        }

        public async Task<ItemsDTO<Order>> GetOrdersGridWithSubgrids(QueryDictionary<string> query)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IGridService>();
                return await service.GetOrdersGridWithSubgrids(query);
            }
        }

        public async Task<ItemsDTO<Order>> GetOrdersGridWithTotals(QueryDictionary<string> query)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IGridService>();
                return await service.GetOrdersGridWithTotals(query);
            }
        }

        public async Task<ItemsDTO<Order>> GetVirtualizedOrdersGridRows(QueryDictionary<string> query)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IGridService>();
                return await service.GetVirtualizedOrdersGrid(query);
            }
        }

        public async Task<ItemsDTO<Order>> OrderColumnsListFilter(QueryDictionary<string> query)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IGridService>();
                return await service.OrderColumnsListFilter(query);
            }
        }

        public async Task<ItemsDTO<Order>> OrderColumnsWithCrud(QueryDictionary<string> query)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IGridService>();
                return await service.OrderColumnsWithCrud(query);
            }
        }

        public async Task<ItemsDTO<Order>> OrderColumnsWithEdit(QueryDictionary<string> query)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IGridService>();
                return await service.OrderColumnsWithEdit(query);
            }
        }

        public async Task<ItemsDTO<Order>> OrderColumnsWithSubgridCrud(QueryDictionary<string> query)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IGridService>();
                return await service.OrderColumnsWithSubgridCrud(query);
            }
        }

        public async Task<Response> Subtract1ToFreight(int id)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IGridService>();
                return await service.Subtract1ToFreight(new Request(id));
            }
        }

        public async Task<ItemsDTO<OrderDetail>> GetOrderDetailsGrid(QueryDictionary<string> query, int orderId)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IGridService>();
                return await service.GetOrderDetailsGrid(new Request(orderId, query));
            }
        }

        public async Task<ItemsDTO<OrderDetail>> GetOrderDetailsGridAllFeatures(QueryDictionary<string> query, int orderId)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IGridService>();
                return await service.GetOrderDetailsGridAllFeatures(new Request(orderId, query));
            }
        }

        public async Task<ItemsDTO<OrderDetail>> GetOrderDetailsGridWithCrud(QueryDictionary<string> query, int orderId)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IGridService>();
                return await service.GetOrderDetailsGridWithCrud(new Request(orderId, query));
            }
        }
    }

    public interface IGridClientService
    {
        Task<ItemsDTO<Order>> GetOrdersGridRows(QueryDictionary<string> query);
        Task<ItemsDTO<Order>> GetOrdersGridordersAutoGenerateColumns(QueryDictionary<string> query);
        Task<ItemsDTO<Order>> GetOrdersGridWithTotals(QueryDictionary<string> query);
        Task<ItemsDTO<Order>> GetOrdersGridWithCount(QueryDictionary<string> query);
        Task<ItemsDTO<Order>> GetOrdersGridExtSorting(QueryDictionary<string> query);
        Task<ItemsDTO<Order>> GetOrdersGridSearchable(QueryDictionary<string> query);
        Task<ItemsDTO<Order>> GetOrdersGridGroupable(QueryDictionary<string> query);
        Task<Order> GetMaxFreight(QueryDictionary<string> query, string clientName);
        Task<Order> GetMinFreight(QueryDictionary<string> query, string clientName);
        Task<ItemsDTO<Order>> GetOrdersGridWithSubgrids(QueryDictionary<string> query);
        Task<ItemsDTO<Order>> OrderColumnsListFilter(QueryDictionary<string> query);
        Task<ItemsDTO<Order>> OrderColumnsWithEdit(QueryDictionary<string> query);
        Task<ItemsDTO<Order>> OrderColumnsWithCrud(QueryDictionary<string> query);
        Task<ItemsDTO<Order>> OrderColumnsWithSubgridCrud(QueryDictionary<string> query);
        Task<ItemsDTO<Order>> GetOrderColumnsWithErrors(QueryDictionary<string> query);
        Task<ItemsDTO<Order>> GetVirtualizedOrdersGridRows(QueryDictionary<string> query);
        Task<ItemsDTO<Order>> GetOrdersGridAllFeatures(QueryDictionary<string> query);
        Task<IEnumerable<string>> GetCustomersNames(QueryDictionary<string> query);
        Task<IEnumerable<SelectItem>> GetAllCustomers();
        Task<IEnumerable<SelectItem>> GetAllCustomers2();
        Task<IEnumerable<SelectItem>> GetAllContacts();
        Task<IEnumerable<SelectItem>> GetAllEmployees();
        Task<IEnumerable<SelectItem>> GetAllShippers();
        Task<IEnumerable<SelectItem>> GetAllProducts();
        Task<ItemsDTO<Customer>> GetCustomersGrid(QueryDictionary<string> query);
        Task<Response> Add1ToFreight(int id);
        Task<Response> Subtract1ToFreight(int id);
        Task<ItemsDTO<Employee>> GetEmployeesGrid(QueryDictionary<string> query);
        Task<ItemsDTO<OrderDetail>> GetOrderDetailsGrid(QueryDictionary<string> query, int orderId);
        Task<ItemsDTO<OrderDetail>> GetOrderDetailsGridWithCrud(QueryDictionary<string> query, int orderId);
        Task<ItemsDTO<OrderDetail>> GetOrderDetailsGridAllFeatures(QueryDictionary<string> query, int orderId);
    }
}
