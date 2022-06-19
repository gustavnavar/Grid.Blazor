using GridBlazorGrpc.Shared.Models;
using GridBlazorGrpc.Shared.Services;
using GridShared;
using GridShared.Utility;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components;
using ProtoBuf.Grpc.Client;
using System.Net.Http;
using System.Threading.Tasks;

namespace GridBlazorGrpc.Client.Services
{
    public class OrderClientService : ICrudDataService<Order>
    {
        private readonly string _baseUri;

        public OrderClientService(NavigationManager navigationManager)
        {
            _baseUri = navigationManager.BaseUri;
        }

        public async Task<Order> Get(params object[] keys)
        {
            int orderId;
            int.TryParse(keys[0].ToString(), out orderId);

            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IOrderService>();
                return await service.Get(new Order { OrderID = orderId });
            }
        }

        public async Task Insert(Order item)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IOrderService>();
                var result = await service.Create(item);
                if (result.Ok)
                {
                    item.OrderID = result.Id;
                }
                else
                {
                    throw new GridException("ORDSRV-01", "Error creating the order: " + result.Message);
                }
            }
        }

        public async Task Update(Order item)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IOrderService>();
                var result = await service.Update(item);
                if (!result.Ok)
                {
                    throw new GridException("ORDSRV-02", "Error updating the order: " + result.Message);
                }
            }
        }

        public async Task Delete(params object[] keys)
        {
            int orderId;
            int.TryParse(keys[0].ToString(), out orderId);

            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IOrderService>();
                var result = await service.Delete(new Order { OrderID = orderId });
                if (!result.Ok)
                {
                    throw new GridException("ORDSRV-03", "Error deleting the order: " + result.Message);
                }
            }
        }
    }
}
