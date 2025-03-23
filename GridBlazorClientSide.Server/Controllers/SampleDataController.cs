using GridBlazorClientSide.Client.ColumnCollections;
using GridBlazorClientSide.Server.Models;
using GridBlazorClientSide.Shared.Models;
using GridCore;
using GridCore.Server;
using GridMvc.Server;
using GridShared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GridBlazorClientSide.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    public class SampleDataController : Controller
    {
        private readonly NorthwindDbContext _context;
        
        public SampleDataController(NorthwindDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetOrdersGrid()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", ColumnCollections.OrderColumns)
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
            return Ok(items);
        }

        [HttpGet]
        public ActionResult GetOrdersGridordersAutoGenerateColumns()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", null)
                    .AutoGenerateColumns()
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = server.ItemsToDisplay;
            return Ok(items);
        }

        [HttpGet]
        public async Task<ActionResult> GetOrdersGridWithTotals()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", ColumnCollections.OrderColumnsWithTotals)
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
            return Ok(items);
        }

        [HttpGet]
        public ActionResult GetOrdersGridWithCount()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridServer<Order>(repository.GetAll().Include(r => r.OrderDetails), Request.Query,
                true, "ordersGrid", ColumnCollections.OrderColumnsCount)
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .Searchable(true, false)
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = server.ItemsToDisplay;
            return Ok(items);
        }

        [HttpGet]
        public async Task<ActionResult> GetOrdersGridSearchable()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", ColumnCollections.OrderColumns)
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .Searchable(true, false, false)
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
            return Ok(items);
        }

        [HttpGet]
        public async Task<ActionResult> GetOrdersGridExtSorting()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", ColumnCollections.OrderColumnsExtSorting)
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .Groupable(true)
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
            return Ok(items);
        }

        [HttpGet]
        public async Task<ActionResult> GetOrdersGridGroupable()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", c => ColumnCollections.OrderColumnsGroupable(c, null))
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .Groupable(true)
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
            return Ok(items);
        }

        [HttpGet]
        public async Task<ActionResult> GetVirtualizedOrdersGrid()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", c => ColumnCollections.VirtualizedOrderColumns(c, null, null))
                    .Sortable()
                    .ExtSortable(true)
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    //.Searchable(true, false, false)
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
            return Ok(items);
        }

        [HttpGet]
        public ActionResult GetMaxFreight(string clientName)
        {
            var repository = new OrdersRepository(_context);
            var server = new GridServer<Order>(repository.GetForClient(clientName), Request.Query, true, "ordersGrid", null)
                .AutoGenerateColumns()
                .Sortable()
                .Filterable()
                .WithMultipleFilters()
                .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");
            return Ok(new Order() { Freight = server.ItemsToDisplay.Items.Max(r => r.Freight) });
        }

        [HttpGet]
        public ActionResult GetMinFreight(string clientName)
        {
            var repository = new OrdersRepository(_context);
            var server = new GridServer<Order>(repository.GetForClient(clientName), Request.Query, true, "ordersGrid", null)
                .AutoGenerateColumns()
                .Sortable()
                .Filterable()
                .WithMultipleFilters()
                .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");
            return Ok(new Order() { Freight = server.ItemsToDisplay.Items.Min(r => r.Freight) });
        }

        [HttpGet]
        public async Task<ActionResult> GetOrdersGridWithSubgrids()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", ColumnCollections.OrderColumnsWithSubgrids)
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());

            return Ok(items);
        }

        [HttpGet]
        public async Task<ActionResult> OrderColumnsListFilter()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", c => ColumnCollections.OrderColumnsListFilter(c, null, null, null))
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
            return Ok(items);
        }

        [HttpGet]
        public async Task<ActionResult> OrderColumnsWithEdit()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", c => ColumnCollections.OrderColumnsWithEdit(c, null, null))
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
            return Ok(items);
        }

        [HttpGet]
        public async Task<ActionResult> OrderColumnsWithCrud()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", c => ColumnCollections.OrderColumnsWithCrud(c, null))
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
            return Ok(items);
        }

        [HttpGet]
        public async Task<ActionResult> OrderColumnsWithSubgridCrud()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", c => ColumnCollections.OrderColumnsWithNestedCrud(c, null, null))
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
            return Ok(items);
        }

        [HttpGet]
        public ActionResult GetOrderColumnsWithErrors()
        {
            var random = new Random();
            if (random.Next(2) == 0)
                return BadRequest();

            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", ColumnCollections.OrderColumns)
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = server.ItemsToDisplay;
            return Ok(items);
        }

        [HttpGet]
        public async Task<ActionResult> GetOrdersGridAllFeatures()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", c => ColumnCollections.OrderColumnsAllFeatures(c, null, null, null))
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .Searchable(true, false)
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());

            return Ok(items);
        }

        [HttpGet]
        public ActionResult GetCustomersNames()
        {
            // get all customer ids in the grid with the current filters
            var orderRepository = new OrdersRepository(_context);
            var server = new GridServer<Order>(orderRepository.GetAll(), Request.Query, true, "ordersGrid", 
                ColumnCollections.OrderColumns);
            var customers = ((GridBase<Order>)server.Grid).GridItems.Where(r => !string.IsNullOrWhiteSpace(r.CustomerID))
                    .Select(r => r.Customer).Distinct()
                    .Select(r => r.CompanyName)
                    .ToList();

            return Ok(customers);
        }

        [HttpGet]
        public ActionResult GetAllCustomers()
        {
            var repository = new CustomersRepository(_context);
            return Ok(repository.GetAll()
                    .Select(r => new SelectItem(r.CustomerID, r.CustomerID + " - " + r.CompanyName))
                    .ToList());
        }

        [HttpGet]
        public ActionResult GetAllCustomers2()
        {
            var repository = new CustomersRepository(_context);
            return Ok(repository.GetAll()
                    .Select(r => new SelectItem(r.CompanyName, r.CompanyName))
                    .ToList());
        }

        [HttpGet]
        public ActionResult GetAllContacts()
        {
            var repository = new CustomersRepository(_context);
            return Ok(repository.GetAll()
                    .Select(r => new SelectItem(r.ContactName, r.ContactName))
                    .ToList());
        }

        [HttpGet]
        public ActionResult GetAllEmployees()
        {
            var repository = new EmployeeRepository(_context);
            return Ok(repository.GetAll()
                    .Select(r => new SelectItem(r.EmployeeID.ToString(), r.EmployeeID.ToString() + " - "
                        + r.FirstName + " " + r.LastName))
                    .ToList());
        }

        [HttpGet]
        public ActionResult GetAllShippers()
        {
            var repository = new ShipperRepository(_context);
            return Ok(repository.GetAll()
                    .Select(r => new SelectItem(r.ShipperID.ToString(), r.ShipperID.ToString() + " - "
                        + r.CompanyName))
                    .ToList());
        }

        [HttpGet]
        public ActionResult GetAllProducts()
        {
            var repository = new ProductRepository(_context);
            return Ok(repository.GetAll()
                    .Select(r => new SelectItem(r.ProductID.ToString(), r.ProductID.ToString() + " - "
                        + r.ProductName))
                    .ToList());
        }

        [HttpGet]
        public async Task<ActionResult> GetOrderDetailsGrid(int OrderId)
        {
            var orderDetails = (new OrderDetailsRepository(_context)).GetForOrder(OrderId);

            var server = new GridServer<OrderDetail>(orderDetails, Request.Query,
                    false, "orderDetailsGrid" + OrderId.ToString(), ColumnCollections.OrderDetailColumns)
                        .WithPaging(10)
                        .Sortable()
                        .Filterable()
                        .WithMultipleFilters()
                        .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
            return Ok(items);
        }

        [HttpGet]
        public async Task<ActionResult> GetOrderDetailsGridWithCrud(int OrderId)
        {
            var orderDetails = (new OrderDetailsRepository(_context)).GetForOrder(OrderId);

            var server = new GridServer<OrderDetail>(orderDetails, Request.Query,
                    false, "orderDetailsGrid" + OrderId.ToString(), c => ColumnCollections.OrderDetailColumnsCrud(c, null))
                        .WithPaging(10)
                        .Sortable()
                        .Filterable()
                        .WithMultipleFilters()
                        .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
            return Ok(items);
        }

        [HttpGet]
        public async Task<ActionResult> GetOrderDetailsGridAllFeatures(int OrderId)
        {
            var orderDetails = (new OrderDetailsRepository(_context)).GetForOrder(OrderId);

            var server = new GridServer<OrderDetail>(orderDetails, Request.Query,
                    false, "orderDetailsGrid" + OrderId.ToString(), c => ColumnCollections.OrderDetailColumnsAllFeatures(c, null))
                        .WithPaging(10)
                        .Sortable()
                        .Filterable()
                        .WithMultipleFilters()
                        .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
            return Ok(items);
        }

        [HttpGet]
        public async Task<ActionResult> GetCustomersGrid()
        {
            var repository = new CustomersRepository(_context);
            IGridServer<Customer> server = new GridServer<Customer>(repository.GetAll(), Request.Query,
                true, "customersGrid", ColumnCollections.CustomersColumns)
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
            return Ok(items);
        }

        [HttpPost]
        public async Task<ActionResult> Add1ToFreight(int id)
        {
            if (ModelState.IsValid)
            {
                var repository = new OrdersRepository(_context);
                try
                {
                    var order = await repository.GetById(id);
                    if (order.Freight.HasValue)
                    {
                        order.Freight += 1;
                        await repository.Update(order);
                        repository.Save();
                    }

                    return NoContent();
                }
                catch (Exception e)
                {
                    return BadRequest(new
                    {
                        message = e.Message.Replace('{', '(').Replace('}', ')')
                    });
                }
            }
            return BadRequest(new
            {
                message = "ModelState is not valid"
            });
        }

        [HttpPost]
        public async Task<ActionResult> Subtract1ToFreight(int id)
        {
            if (ModelState.IsValid)
            {
                var repository = new OrdersRepository(_context);
                try
                {
                    var order = await repository.GetById(id);
                    if (order.Freight.HasValue)
                    {
                        order.Freight -= 1;
                        await repository.Update(order);
                        repository.Save();
                    }

                    return NoContent();
                }
                catch (Exception e)
                {
                    return BadRequest(new
                    {
                        message = e.Message.Replace('{', '(').Replace('}', ')')
                    });
                }
            }
            return BadRequest(new
            {
                message = "ModelState is not valid"
            });
        }

        [HttpGet]
        public async Task<ActionResult> GetEmployeesGrid()
        {
            var repository = new EmployeeRepository(_context);
            IGridServer<Employee> server = new GridServer<Employee>(repository.GetAll(), Request.Query,
                true, "employeesGrid", ColumnCollections.EmployeeColumns)
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
            return Ok(items);
        }

        [HttpPost]
        public async Task<ActionResult> SetEmployeePhoto([FromBody] Employee employee)
        {
            if (ModelState.IsValid)
            {
                var repository = new EmployeeRepository(_context);
                try
                {
                    await repository.UpdatePhoto(employee.EmployeeID, employee.Base64String);
                    repository.Save();

                    return NoContent();
                }
                catch (Exception e)
                {
                    return BadRequest(new
                    {
                        message = e.Message.Replace('{', '(').Replace('}', ')')
                    });
                }
            }
            return BadRequest(new
            {
                message = "ModelState is not valid"
            });
        }
    }
}
