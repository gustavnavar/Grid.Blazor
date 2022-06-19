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
    public class CustomerClientService : ICrudDataService<Customer>
    {
        private readonly string _baseUri;

        public CustomerClientService(NavigationManager navigationManager)
        {
            _baseUri = navigationManager.BaseUri;
        }

        public async Task<Customer> Get(params object[] keys)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<ICustomerService>();
                return await service.Get(new Customer { CustomerID = keys[0].ToString() });
            }
        }

        public async Task Insert(Customer item)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<ICustomerService>();
                var result = await service.Create(item);
                if (result.Ok)
                {
                    item.CustomerID = result.Code;
                }
                else
                {
                    throw new GridException("CUSSRV-01", "Error creating the customer: " + result.Message);
                }
            }
        }

        public async Task Update(Customer item)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<ICustomerService>();
                var result = await service.Update(item);
                if (!result.Ok)
                {
                    throw new GridException("CUSSRV-02", "Error updating the customer: " + result.Message);
                }
            }
        }

        public async Task Delete(params object[] keys)
        {
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<ICustomerService>();
                var result = await service.Delete(new Customer { CustomerID = keys[0].ToString() });
                if (!result.Ok)
                {
                    throw new GridException("CUSSRV-03", "Error deleting the customer: " + result.Message);
                }
            }
        }
    }
}
