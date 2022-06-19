using GridBlazorGrpc.Shared.Models;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace GridBlazorGrpc.Shared.Services
{
    [ServiceContract]
    public interface IOrderDetailService
    {
        ValueTask<IEnumerable<OrderDetail>> GetAll();
        ValueTask<Response> Create(OrderDetail orderDetail);
        ValueTask<OrderDetail> Get(OrderDetail orderDetail);
        ValueTask<Response> Update(OrderDetail orderDetail);
        ValueTask<Response> Delete(OrderDetail orderDetail);
    }
}
