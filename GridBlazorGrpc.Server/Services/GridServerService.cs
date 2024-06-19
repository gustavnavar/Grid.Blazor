using GridBlazorGrpc.Client.ColumnCollections;
using GridBlazorGrpc.Server.Models;
using GridBlazorGrpc.Shared.Models;
using GridBlazorGrpc.Shared.Services;
using GridCore;
using GridCore.Server;
using GridShared;
using GridShared.Utility;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GridBlazorGrpc.Server.Services
{
    public class GridServerService : IGridService
    {
        private readonly NorthwindDbContext _context;
        
        public GridServerService(NorthwindDbContext context)
        {
            _context = context;
        }

        public async ValueTask<ItemsDTO<Order>> GetOrdersGrid(QueryDictionary<string> query)
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridCoreServer<Order>(repository.GetAll(), query,
                true, "ordersGrid", ColumnCollections.OrderColumns)
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
            return items;
        }

        public async ValueTask<ItemsDTO<Order>> GetOrdersGridordersAutoGenerateColumns(QueryDictionary<string> query)
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridCoreServer<Order>(repository.GetAll(), query,
                true, "ordersGrid", null)
                    .AutoGenerateColumns()
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
            return items;
        }

        public async ValueTask<ItemsDTO<Order>> GetOrdersGridWithTotals(QueryDictionary<string> query)
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridCoreServer<Order>(repository.GetAll(), query,
                true, "ordersGrid", ColumnCollections.OrderColumnsWithTotals)
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
            return items;
        }

        public async ValueTask<ItemsDTO<Order>> GetOrdersGridWithCount(QueryDictionary<string> query)
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridCoreServer<Order>(repository.GetAll().Include(r => r.OrderDetails), query,
                true, "ordersGrid", ColumnCollections.OrderColumnsCount)
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .Searchable(true, false)
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
            return items;
        }

        public async ValueTask<ItemsDTO<Order>> GetOrdersGridSearchable(QueryDictionary<string> query)
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridCoreServer<Order>(repository.GetAll(), query,
                true, "ordersGrid", ColumnCollections.OrderColumns)
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .Searchable(true, false, false)
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
            return items;
        }

        public async ValueTask<ItemsDTO<Order>> GetOrdersGridExtSorting(QueryDictionary<string> query)
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridCoreServer<Order>(repository.GetAll(), query,
                true, "ordersGrid", ColumnCollections.OrderColumnsExtSorting)
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .Groupable(true)
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
            return items;
        }

        public async ValueTask<ItemsDTO<Order>> GetOrdersGridGroupable(QueryDictionary<string> query)
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridCoreServer<Order>(repository.GetAll(), query,
                true, "ordersGrid", c => ColumnCollections.OrderColumnsGroupable(c, null))
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .Groupable(true)
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
            return items;
        }

        public async ValueTask<Order> GetMaxFreight(Request request)
        {
            var repository = new OrdersRepository(_context);
            var server = new GridCoreServer<Order>(repository.GetForClient(request.Name), request.Query, true, "ordersGrid", null)
                .AutoGenerateColumns()
                .Sortable()
                .Filterable()
                .WithMultipleFilters()
                .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");
            return await Task.FromResult(new Order() { Freight = server.ItemsToDisplay.Items.Max(r => r.Freight) });
        }

        public async ValueTask<Order> GetMinFreight(Request request)
        {
            var repository = new OrdersRepository(_context);
            var server = new GridCoreServer<Order>(repository.GetForClient(request.Name), request.Query, true, "ordersGrid", null)
                .AutoGenerateColumns()
                .Sortable()
                .Filterable()
                .WithMultipleFilters()
                .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");
            return await Task.FromResult(new Order() { Freight = server.ItemsToDisplay.Items.Min(r => r.Freight) });
        }

        public async ValueTask<ItemsDTO<Order>> GetOrdersGridWithSubgrids(QueryDictionary<string> query)
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridCoreServer<Order>(repository.GetAll(), query,
                true, "ordersGrid", ColumnCollections.OrderColumnsWithSubgrids)
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());

            return items;
        }

        public async ValueTask<ItemsDTO<Order>> OrderColumnsListFilter(QueryDictionary<string> query)
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridCoreServer<Order>(repository.GetAll(), query,
                true, "ordersGrid", c => ColumnCollections.OrderColumnsListFilter(c, null, null, null))
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
            return items;
        }

        public async ValueTask<ItemsDTO<Order>> OrderColumnsWithEdit(QueryDictionary<string> query)
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridCoreServer<Order>(repository.GetAll(), query,
                true, "ordersGrid", c => ColumnCollections.OrderColumnsWithEdit(c, null, null))
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
            return items;
        }

        public async ValueTask<ItemsDTO<Order>> OrderColumnsWithCrud(QueryDictionary<string> query)
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridCoreServer<Order>(repository.GetAll(), query,
                true, "ordersGrid", c => ColumnCollections.OrderColumnsWithCrud(c, null, null, null))
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
            return items;
        }

        public async ValueTask<ItemsDTO<Order>> OrderColumnsWithSubgridCrud(QueryDictionary<string> query)
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridCoreServer<Order>(repository.GetAll(), query,
                true, "ordersGrid", c => ColumnCollections.OrderColumnsWithNestedCrud(c, null, null, null, null))
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
            return items;
        }

        public async ValueTask<ItemsDTO<Order>> GetOrderColumnsWithErrors(QueryDictionary<string> query)
        {
            var random = new Random();
            if (random.Next(2) == 0)
                return null;

            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridCoreServer<Order>(repository.GetAll(), query,
                true, "ordersGrid", ColumnCollections.OrderColumns)
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
            return items;
        }

        public async ValueTask<ItemsDTO<Order>> GetVirtualizedOrdersGrid(QueryDictionary<string> query)
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridCoreServer<Order>(repository.GetAll(), query,
                true, "ordersGrid", c => ColumnCollections.VirtualizedOrderColumns(c, null, null, null, null))
                    .Sortable()
                    .ExtSortable(true)
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    //.Searchable(true, false, false)
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
            return items;
        }

        public async ValueTask<ItemsDTO<Order>> GetOrdersGridAllFeatures(QueryDictionary<string> query)
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridCoreServer<Order>(repository.GetAllWithEmployee(), query,
                true, "ordersGrid", c => ColumnCollections.OrderColumnsAllFeatures(c, null, null, null, null, null))
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .Searchable(true, false)
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());

            return items;
        }

        public async ValueTask<IEnumerable<string>> GetCustomersNames(QueryDictionary<string> query)
        {
            // get all customer ids in the grid with the current filters
            var orderRepository = new OrdersRepository(_context);
            var server = new GridCoreServer<Order>(orderRepository.GetAll(), query, true, "ordersGrid", 
                ColumnCollections.OrderColumns);
            return await ((GridBase<Order>)server.Grid).GridItems.Where(r => !string.IsNullOrWhiteSpace(r.CustomerID))
                    .Select(r => r.Customer).Distinct()
                    .Select(r => r.CompanyName)
                    .ToListAsync();
        }

        public async ValueTask<IEnumerable<SelectItem>> GetAllCustomers()
        {
            var repository = new CustomersRepository(_context);
            return await repository.GetAll()
                    .Select(r => new SelectItem(r.CustomerID, r.CustomerID + " - " + r.CompanyName))
                    .ToListAsync();
        }

        public async ValueTask<IEnumerable<SelectItem>> GetAllCustomers2()
        {
            var repository = new CustomersRepository(_context);
            return await repository.GetAll()
                    .Select(r => new SelectItem(r.CompanyName, r.CompanyName))
                    .ToListAsync();
        }

        public async ValueTask<IEnumerable<SelectItem>> GetAllContacts()
        {
            var repository = new CustomersRepository(_context);
            return await repository.GetAll()
                    .Select(r => new SelectItem(r.ContactName, r.ContactName))
                    .ToListAsync();
        }

        public async ValueTask<IEnumerable<SelectItem>> GetAllEmployees()
        {
            var repository = new EmployeeRepository(_context);
            return await repository.GetAll()
                    .Select(r => new SelectItem(r.EmployeeID.ToString(), r.EmployeeID.ToString() + " - "
                        + r.FirstName + " " + r.LastName))
                    .ToListAsync();
        }

        public async ValueTask<IEnumerable<SelectItem>> GetAllShippers()
        {
            var repository = new ShipperRepository(_context);
            return await repository.GetAll()
                    .Select(r => new SelectItem(r.ShipperID.ToString(), r.ShipperID.ToString() + " - "
                        + r.CompanyName))
                    .ToListAsync();
        }

        public async ValueTask<IEnumerable<SelectItem>> GetAllProducts()
        {
            var repository = new ProductRepository(_context);
            return await repository.GetAll()
                    .Select(r => new SelectItem(r.ProductID.ToString(), r.ProductID.ToString() + " - "
                        + r.ProductName))
                    .ToListAsync();
        }

        public async ValueTask<ItemsDTO<Customer>> GetCustomersGrid(QueryDictionary<string> query)
        {
            var repository = new CustomersRepository(_context);
            IGridServer<Customer> server = new GridCoreServer<Customer>(repository.GetAll(), query,
                true, "customersGrid", ColumnCollections.CustomersColumns)
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
            return items;
        }

        public async ValueTask<Response> Add1ToFreight(Request request)
        {
            var repository = new OrdersRepository(_context);
            try
            {
                var order = await repository.GetById(request.Id);
                if (order.Freight.HasValue)
                {
                    order.Freight += 1;
                    await repository.Update(order);
                    repository.Save();
                }

                return new Response(true);
            }
            catch (Exception e)
            {
                return new Response(e);
            }
        }

        public async ValueTask<Response> Subtract1ToFreight(Request request)
        {
            var repository = new OrdersRepository(_context);
            try
            {
                var order = await repository.GetById(request.Id);
                if (order.Freight.HasValue)
                {
                    order.Freight -= 1;
                    await repository.Update(order);
                    repository.Save();
                }

                return new Response(true);
            }
            catch (Exception e)
            {
                return new Response(e);
            }
        }

        public async ValueTask<ItemsDTO<Employee>> GetEmployeesGrid(QueryDictionary<string> query)
        {
            var repository = new EmployeeRepository(_context);
            IGridServer<Employee> server = new GridCoreServer<Employee>(repository.GetAll(), query,
                true, "employeesGrid", ColumnCollections.EmployeeColumns)
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
            return items;
        }

        public async ValueTask<Response> SetEmployeePhoto(Employee employee)
        {
            var repository = new EmployeeRepository(_context);
            try
            {
                await repository.UpdatePhoto(employee.EmployeeID, employee.Base64String);
                repository.Save();

                return new Response(true);
            }
            catch (Exception e)
            {
                return new Response(e);
            }
        }

        public async ValueTask<ItemsDTO<OrderDetail>> GetOrderDetailsGrid(Request request)
        {
            var orderDetails = (new OrderDetailsRepository(_context)).GetForOrder(request.Id);

            var server = new GridCoreServer<OrderDetail>(orderDetails, request.Query,
                    false, "orderDetailsGrid" + request.Id.ToString(), ColumnCollections.OrderDetailColumns)
                        .WithPaging(10)
                        .Sortable()
                        .Filterable()
                        .WithMultipleFilters()
                        .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
            return items;
        }

        public async ValueTask<ItemsDTO<OrderDetail>> GetOrderDetailsGridWithCrud(Request request)
        {
            var orderDetails = (new OrderDetailsRepository(_context)).GetForOrder(request.Id);

            var server = new GridCoreServer<OrderDetail>(orderDetails, request.Query,
                    false, "orderDetailsGrid" + request.Id.ToString(), c => ColumnCollections.OrderDetailColumnsCrud(c, null))
                        .WithPaging(10)
                        .Sortable()
                        .Filterable()
                        .WithMultipleFilters()
                        .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
            return items;
        }

        public async ValueTask<ItemsDTO<OrderDetail>> GetOrderDetailsGridAllFeatures(Request request)
        {
            var orderDetails = (new OrderDetailsRepository(_context)).GetForOrder(request.Id);

            var server = new GridCoreServer<OrderDetail>(orderDetails, request.Query,
                    false, "orderDetailsGrid" + request.Id.ToString(), c => ColumnCollections.OrderDetailColumnsAllFeatures(c, null))
                        .WithPaging(10)
                        .Sortable()
                        .Filterable()
                        .WithMultipleFilters()
                        .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
            return items;
        }
    }
}
