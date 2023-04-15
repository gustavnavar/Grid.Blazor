using GridBlazorGrpc.Shared.Models;
using GridShared;
using GridShared.Utility;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace GridBlazorGrpc.Shared.Services
{
    [ServiceContract]
    public interface IGridService
    {
        ValueTask<ItemsDTO<Order>> GetOrdersGrid(QueryDictionary<string> query);
        ValueTask<ItemsDTO<Order>> GetOrdersGridordersAutoGenerateColumns(QueryDictionary<string> query);
        ValueTask<ItemsDTO<Order>> GetOrdersGridWithTotals(QueryDictionary<string> query);
        ValueTask<ItemsDTO<Order>> GetOrdersGridWithCount(QueryDictionary<string> query);
        ValueTask<ItemsDTO<Order>> GetOrdersGridExtSorting(QueryDictionary<string> query);
        ValueTask<ItemsDTO<Order>> GetOrdersGridSearchable(QueryDictionary<string> query);
        ValueTask<ItemsDTO<Order>> GetOrdersGridGroupable(QueryDictionary<string> query);
        ValueTask<Order> GetMaxFreight(Request request);
        ValueTask<Order> GetMinFreight(Request request);
        ValueTask<ItemsDTO<Order>> GetOrdersGridWithSubgrids(QueryDictionary<string> query);
        ValueTask<ItemsDTO<Order>> OrderColumnsListFilter(QueryDictionary<string> query);
        ValueTask<ItemsDTO<Order>> OrderColumnsWithEdit(QueryDictionary<string> query);
        ValueTask<ItemsDTO<Order>> OrderColumnsWithCrud(QueryDictionary<string> query);
        ValueTask<ItemsDTO<Order>> OrderColumnsWithSubgridCrud(QueryDictionary<string> query);
        ValueTask<ItemsDTO<Order>> GetOrderColumnsWithErrors(QueryDictionary<string> query);
        ValueTask<ItemsDTO<Order>> GetVirtualizedOrdersGrid(QueryDictionary<string> query);
        ValueTask<ItemsDTO<Order>> GetOrdersGridAllFeatures(QueryDictionary<string> query);
        ValueTask<IEnumerable<string>> GetCustomersNames();
        ValueTask<IEnumerable<SelectItem>> GetAllCustomers();
        ValueTask<IEnumerable<SelectItem>> GetAllCustomers2();
        ValueTask<IEnumerable<SelectItem>> GetAllContacts();
        ValueTask<IEnumerable<SelectItem>> GetAllEmployees();
        ValueTask<IEnumerable<SelectItem>> GetAllShippers();
        ValueTask<IEnumerable<SelectItem>> GetAllProducts();
        ValueTask<ItemsDTO<Customer>> GetCustomersGrid(QueryDictionary<string> query);
        ValueTask<Response> Add1ToFreight(Request request);
        ValueTask<Response> Subtract1ToFreight(Request request);
        ValueTask<ItemsDTO<Employee>> GetEmployeesGrid(QueryDictionary<string> query);
        ValueTask<Response> SetEmployeePhoto(Employee employee);
        ValueTask<ItemsDTO<OrderDetail>> GetOrderDetailsGrid(Request request);
        ValueTask<ItemsDTO<OrderDetail>> GetOrderDetailsGridWithCrud(Request request);
        ValueTask<ItemsDTO<OrderDetail>> GetOrderDetailsGridAllFeatures(Request request);
    }
}
