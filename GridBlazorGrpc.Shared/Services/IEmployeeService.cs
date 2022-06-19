using GridBlazorGrpc.Shared.Models;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace GridBlazorGrpc.Shared.Services
{
    [ServiceContract]
    public interface IEmployeeService
    {
        ValueTask<IEnumerable<Employee>> GetAll();
        ValueTask<Response> Create(Employee employee);
        ValueTask<Employee> Get(Employee employee);
        ValueTask<Response> Update(Employee employee);
        ValueTask<Response> Delete(Employee employee);
    }
}
