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
    public class EmployeeClientService : ICrudDataService<Employee>
    {
        private readonly string _baseUri;

        public EmployeeClientService(NavigationManager navigationManager)
        {
            _baseUri = navigationManager.BaseUri;
        }

        public async Task<Employee> Get(params object[] keys)
        {
            int employeeId;
            int.TryParse(keys[0].ToString(), out employeeId);

            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IEmployeeService>();
                return await service.Get(new Employee { EmployeeID = employeeId });
            }
        }

        public async Task Insert(Employee item)
        {
            item.Base64String = null;
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IEmployeeService>();
                var result = await service.Create(item);
                if (result.Ok)
                {
                    item.EmployeeID = result.Id;
                }
                else
                {
                    throw new GridException("EMPSRV-01", "Error creating the employee: " + result.Message);
                }
            }
        }

        public async Task Update(Employee item)
        {
            item.Base64String = null;
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IEmployeeService>();
                var result = await service.Update(item);
                if (!result.Ok)
                {
                    throw new GridException("EMPSRV-02", "Error updating the employee: " + result.Message);
                }
            }
        }

        public async Task Delete(params object[] keys)
        {
            int employeeId;
            int.TryParse(keys[0].ToString(), out employeeId);
            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());
            using (var channel = GrpcChannel.ForAddress(_baseUri, new GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
            {
                var service = channel.CreateGrpcService<IEmployeeService>();
                var result = await service.Delete(new Employee { EmployeeID = employeeId });
                if (!result.Ok)
                {
                    throw new GridException("EMPSRV-03", "Error deleting the employee: " + result.Message);
                }
            }
        }
    }
}
