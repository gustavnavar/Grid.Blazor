using GridBlazorGrpc.Shared.Models;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace GridBlazorGrpc.Shared.Services
{
    [ServiceContract]
    public interface IOrderService
    {
        ValueTask<IEnumerable<Order>> GetAll();
        ValueTask<Response> Create(Order order);
        ValueTask<Order> Get(Order order);
        ValueTask<Response> Update(Order order);
        ValueTask<Response> Delete(Order order);
    }
}
