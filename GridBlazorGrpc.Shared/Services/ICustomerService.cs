using GridBlazorGrpc.Shared.Models;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace GridBlazorGrpc.Shared.Services
{
    [ServiceContract]
    public interface ICustomerService
    {
        ValueTask<IEnumerable<Customer>> GetAll();
        ValueTask<Response> Create(Customer customer);
        ValueTask<Customer> Get(Customer customer);
        ValueTask<Response> Update(Customer customer);
        ValueTask<Response> Delete(Customer customer);
    }
}
