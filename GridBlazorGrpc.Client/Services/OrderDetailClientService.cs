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
    public class OrderDetailClientService : ICrudDataService<OrderDetail>
    {
        private readonly string _baseUri;

        public OrderDetailClientService(NavigationManager navigationManager)
        {
            _baseUri = navigationManager.BaseUri;
        }

        public async Task<OrderDetail> Get(params object[] keys)
        {
            int orderId;
            int.TryParse(keys[0].ToString(), out orderId);
            int productId;
            int.TryParse(keys[1].ToString(), out productId);
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IOrderDetailService>();
                return await service.Get(new OrderDetail { OrderID = orderId, ProductID = productId });
            }
        }

        public async Task Insert(OrderDetail item)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IOrderDetailService>();
                var result = await service.Create(item);
                if (!result.Ok)
                {
                    throw new GridException("DETSRV-01", "Error creating the order detail: " + result.Message);
                }
            }
        }

        public async Task Update(OrderDetail item)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IOrderDetailService>();
                var result = await service.Update(item);
                if (!result.Ok)
                {
                    throw new GridException("DETSRV-02", "Error updating the order detail: " + result.Message);
                }
            }
        }

        public async Task Delete(params object[] keys)
        {
            int orderId;
            int.TryParse(keys[0].ToString(), out orderId);
            int productId;
            int.TryParse(keys[1].ToString(), out productId);

            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IOrderDetailService>();
                var result = await service.Delete(new OrderDetail { OrderID = orderId, ProductID = productId });
                if (!result.Ok)
                {
                    throw new GridException("DETSRV-03", "Error deleting the order detail: " + result.Message);
                }
            }
        }
    }
}
