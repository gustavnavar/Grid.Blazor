using GridBlazorClientSide.Client.ColumnCollections;
using GridBlazorClientSide.Server.Models;
using GridBlazorClientSide.Shared.Models;
using GridMvc.Server;
using GridShared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GridBlazorClientSide.Server.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        private readonly NorthwindDbContext _context;
        
        public SampleDataController(NorthwindDbContext context)
        {
            _context = context;
        }

        [HttpGet("[action]")]
        public ActionResult GetOrdersGrid()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", ColumnCollections.OrderColumns)
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount();

            var items = server.ItemsToDisplay;
            return Ok(items);
        }

        [HttpGet("[action]")]
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
                    .WithGridItemsCount();

            var items = server.ItemsToDisplay;
            return Ok(items);
        }

        [HttpGet("[action]")]
        public ActionResult GetOrdersGridWithTotals()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", ColumnCollections.OrderColumnsWithTotals)
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount();

            var items = server.ItemsToDisplay;
            return Ok(items);
        }

        [HttpGet("[action]")]
        public ActionResult GetOrdersGridSearchable()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", ColumnCollections.OrderColumns)
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .Searchable(true, false, false);

            var items = server.ItemsToDisplay;
            return Ok(items);
        }

        [HttpGet("[action]")]
        public ActionResult GetOrdersGridGroupable()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", ColumnCollections.OrderColumnsGroupable)
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .Groupable(true);

            var items = server.ItemsToDisplay;
            return Ok(items);
        }

        [HttpGet("[action]")]
        public ActionResult GetOrdersGridWithSubgrids()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", ColumnCollections.OrderColumnsWithSubgrids)
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters();

            var items = server.ItemsToDisplay;

            return Ok(items);
        }

        [HttpGet("[action]")]
        public ActionResult OrderColumnsWithEdit()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", c => ColumnCollections.OrderColumnsWithEdit(c, null, null))
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount();

            var items = server.ItemsToDisplay;
            return Ok(items);
        }

        [HttpGet("[action]")]
        public ActionResult OrderColumnsWithCrud()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", c => ColumnCollections.OrderColumnsWithCrud(c, null))
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount();

            var items = server.ItemsToDisplay;
            return Ok(items);
        }

        [HttpGet("[action]")]
        public ActionResult OrderColumnsWithSubgridCrud()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", c => ColumnCollections.OrderColumnsWithNestedCrud(c, null, null))
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount();

            var items = server.ItemsToDisplay;
            return Ok(items);
        }

        [HttpGet("[action]")]
        public ActionResult GetOrdersGridAllFeatures()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", c => ColumnCollections.OrderColumnsAllFeatures(c, null, null, null))
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .Searchable(true, false);

            var items = server.ItemsToDisplay;

            return Ok(items);
        }

        [HttpGet("[action]")]
        public ActionResult GetCustomersNames()
        {
            var repository = new CustomersRepository(_context);
            return Ok(repository.GetAll().Select(r => r.CompanyName));
        }

        [HttpGet("[action]")]
        public ActionResult GetAllCustomers()
        {
            var repository = new CustomersRepository(_context);
            return Ok(repository.GetAll()
                    .Select(r => new SelectItem(r.CustomerID, r.CustomerID + " - " + r.CompanyName))
                    .ToList());
        }

        [HttpGet("[action]")]
        public ActionResult GetAllEmployees()
        {
            var repository = new EmployeeRepository(_context);
            return Ok(repository.GetAll()
                    .Select(r => new SelectItem(r.EmployeeID.ToString(), r.EmployeeID.ToString() + " - "
                        + r.FirstName + " " + r.LastName))
                    .ToList());
        }

        [HttpGet("[action]")]
        public ActionResult GetAllShippers()
        {
            var repository = new ShipperRepository(_context);
            return Ok(repository.GetAll()
                    .Select(r => new SelectItem(r.ShipperID.ToString(), r.ShipperID.ToString() + " - "
                        + r.CompanyName))
                    .ToList());
        }

        [HttpGet("[action]")]
        public ActionResult GetAllProducts()
        {
            var repository = new ProductRepository(_context);
            return Ok(repository.GetAll()
                    .Select(r => new SelectItem(r.ProductID.ToString(), r.ProductID.ToString() + " - "
                        + r.ProductName))
                    .ToList());
        }

        [HttpGet("[action]")]
        public ActionResult GetOrderDetailsGrid(int OrderId)
        {
            var orderDetails = (new OrderDetailsRepository(_context)).GetForOrder(OrderId);

            var server = new GridServer<OrderDetail>(orderDetails, Request.Query,
                    false, "orderDetailsGrid" + OrderId.ToString(), ColumnCollections.OrderDetailColumns)
                        .WithPaging(10)
                        .Sortable()
                        .Filterable()
                        .WithMultipleFilters();

            var items = server.ItemsToDisplay;
            return Ok(items);
        }

        [HttpGet("[action]")]
        public ActionResult GetOrderDetailsGridWithCrud(int OrderId)
        {
            var orderDetails = (new OrderDetailsRepository(_context)).GetForOrder(OrderId);

            var server = new GridServer<OrderDetail>(orderDetails, Request.Query,
                    false, "orderDetailsGrid" + OrderId.ToString(), c => ColumnCollections.OrderDetailColumnsCrud(c, null))
                        .WithPaging(10)
                        .Sortable()
                        .Filterable()
                        .WithMultipleFilters();

            var items = server.ItemsToDisplay;
            return Ok(items);
        }

        [HttpGet("[action]")]
        public ActionResult GetOrderDetailsGridAllFeatures(int OrderId)
        {
            var orderDetails = (new OrderDetailsRepository(_context)).GetForOrder(OrderId);

            var server = new GridServer<OrderDetail>(orderDetails, Request.Query,
                    false, "orderDetailsGrid" + OrderId.ToString(), c => ColumnCollections.OrderDetailColumnsAllFeatures(c, null))
                        .WithPaging(10)
                        .Sortable()
                        .Filterable()
                        .WithMultipleFilters();

            var items = server.ItemsToDisplay;
            return Ok(items);
        }

        [HttpGet("[action]")]
        public ActionResult GetCustomersGrid()
        {
            var repository = new CustomersRepository(_context);
            IGridServer<Customer> server = new GridServer<Customer>(repository.GetAll(), Request.Query,
                true, "customersGrid", ColumnCollections.CustomersColumns)
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount();

            var items = server.ItemsToDisplay;
            return Ok(items);
        }

        [HttpPost("[action]")]
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

        [HttpPost("[action]")]
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

        [HttpGet("[action]")]
        public ActionResult GetemployeesGrid()
        {
            var repository = new EmployeeRepository(_context);
            IGridServer<Employee> server = new GridServer<Employee>(repository.GetAll(), Request.Query,
                true, "employeesGrid", ColumnCollections.EmployeeColumns)
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount();

            var items = server.ItemsToDisplay;
            return Ok(items);
        }

        [HttpPost("[action]")]
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
