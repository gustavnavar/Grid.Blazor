using GridBlazorClientSide.Client.ColumnCollections;
using GridBlazorClientSide.Client.Pages;
using GridBlazorClientSide.Server.Models;
using GridBlazorClientSide.Shared.Models;
using GridCore.Server;
using GridShared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
            IGridServer<Order> server = new GridCoreServer<Order>(repository.GetAll(), Request.Query,
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

        [HttpGet("[action]")]
        public ActionResult GetOrdersGridordersAutoGenerateColumns()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridCoreServer<Order>(repository.GetAll(), Request.Query,
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

        [HttpGet("[action]")]
        public ActionResult GetOrdersGridWithTotals()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridCoreServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", ColumnCollections.OrderColumnsWithTotals)
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = server.ItemsToDisplay;
            return Ok(items);
        }

        [HttpGet("[action]")]
        public ActionResult GetOrdersGridWithCount()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridCoreServer<Order>(repository.GetAll().Include(r => r.OrderDetails), Request.Query,
                true, "ordersGrid", ColumnCollections.OrderColumnsCount)
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = server.ItemsToDisplay;
            return Ok(items);
        }

        [HttpGet("[action]")]
        public ActionResult GetOrdersGridSearchable()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridCoreServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", ColumnCollections.OrderColumns)
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .Searchable(true, false, false)
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = server.ItemsToDisplay;
            return Ok(items);
        }

        [HttpGet("[action]")]
        public ActionResult GetOrdersGridExtSorting()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridCoreServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", ColumnCollections.OrderColumnsExtSorting)
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .Groupable(true)
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = server.ItemsToDisplay;
            return Ok(items);
        }

        [HttpGet("[action]")]
        public ActionResult GetOrdersGridGroupable()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridCoreServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", c => ColumnCollections.OrderColumnsGroupable(c, null))
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .Groupable(true)
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = server.ItemsToDisplay;
            return Ok(items);
        }

        [HttpGet("[action]")]
        public ActionResult GetMaxFreight(string clientName)
        {
            var repository = new OrdersRepository(_context);
            var server = new GridCoreServer<Order>(repository.GetForClient(clientName), Request.Query, true, "ordersGrid", null)
                .AutoGenerateColumns()
                .Sortable()
                .Filterable()
                .WithMultipleFilters()
                .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");
            return Ok(new Order() { Freight = server.ItemsToDisplay.Items.Max(r => r.Freight) });
        }

        [HttpGet("[action]")]
        public ActionResult GetMinFreight(string clientName)
        {
            var repository = new OrdersRepository(_context);
            var server = new GridCoreServer<Order>(repository.GetForClient(clientName), Request.Query, true, "ordersGrid", null)
                .AutoGenerateColumns()
                .Sortable()
                .Filterable()
                .WithMultipleFilters()
                .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");
            return Ok(new Order() { Freight = server.ItemsToDisplay.Items.Min(r => r.Freight) });
        }

        [HttpGet("[action]")]
        public ActionResult GetOrdersGridWithSubgrids()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridCoreServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", ColumnCollections.OrderColumnsWithSubgrids)
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = server.ItemsToDisplay;

            return Ok(items);
        }

        [HttpGet("[action]")]
        public ActionResult OrderColumnsWithEdit()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridCoreServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", c => ColumnCollections.OrderColumnsWithEdit(c, null, null))
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = server.ItemsToDisplay;
            return Ok(items);
        }

        [HttpGet("[action]")]
        public ActionResult OrderColumnsWithCrud()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridCoreServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", c => ColumnCollections.OrderColumnsWithCrud(c, null))
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = server.ItemsToDisplay;
            return Ok(items);
        }

        [HttpGet("[action]")]
        public ActionResult OrderColumnsWithSubgridCrud()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridCoreServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", c => ColumnCollections.OrderColumnsWithNestedCrud(c, null, null))
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = server.ItemsToDisplay;
            return Ok(items);
        }

        [HttpGet("[action]")]
        public ActionResult GetOrderColumnsWithErrors()
        {
            var random = new Random();
            if (random.Next(2) == 0)
                return BadRequest();

            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridCoreServer<Order>(repository.GetAll(), Request.Query,
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

        [HttpGet("[action]")]
        public ActionResult GetOrdersGridAllFeatures()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridCoreServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", c => ColumnCollections.OrderColumnsAllFeatures(c, null, null, null))
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .Searchable(true, false)
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

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

            var server = new GridCoreServer<OrderDetail>(orderDetails, Request.Query,
                    false, "orderDetailsGrid" + OrderId.ToString(), ColumnCollections.OrderDetailColumns)
                        .WithPaging(10)
                        .Sortable()
                        .Filterable()
                        .WithMultipleFilters()
                        .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = server.ItemsToDisplay;
            return Ok(items);
        }

        [HttpGet("[action]")]
        public ActionResult GetOrderDetailsGridWithCrud(int OrderId)
        {
            var orderDetails = (new OrderDetailsRepository(_context)).GetForOrder(OrderId);

            var server = new GridCoreServer<OrderDetail>(orderDetails, Request.Query,
                    false, "orderDetailsGrid" + OrderId.ToString(), c => ColumnCollections.OrderDetailColumnsCrud(c, null))
                        .WithPaging(10)
                        .Sortable()
                        .Filterable()
                        .WithMultipleFilters()
                        .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = server.ItemsToDisplay;
            return Ok(items);
        }

        [HttpGet("[action]")]
        public ActionResult GetOrderDetailsGridAllFeatures(int OrderId)
        {
            var orderDetails = (new OrderDetailsRepository(_context)).GetForOrder(OrderId);

            var server = new GridCoreServer<OrderDetail>(orderDetails, Request.Query,
                    false, "orderDetailsGrid" + OrderId.ToString(), c => ColumnCollections.OrderDetailColumnsAllFeatures(c, null))
                        .WithPaging(10)
                        .Sortable()
                        .Filterable()
                        .WithMultipleFilters()
                        .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = server.ItemsToDisplay;
            return Ok(items);
        }

        [HttpGet("[action]")]
        public ActionResult GetCustomersGrid()
        {
            var repository = new CustomersRepository(_context);
            IGridServer<Customer> server = new GridCoreServer<Customer>(repository.GetAll(), Request.Query,
                true, "customersGrid", ColumnCollections.CustomersColumns)
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

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
        public ActionResult GetEmployeesGrid()
        {
            var repository = new EmployeeRepository(_context);
            IGridServer<Employee> server = new GridCoreServer<Employee>(repository.GetAll(), Request.Query,
                true, "employeesGrid", ColumnCollections.EmployeeColumns)
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

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

        [HttpGet("[action]")]
        public ActionResult GetTrucksGrid()
        {
            var repository = new EmployeeRepository(_context);
            IGridServer<Truck> server = new GridCoreServer<Truck>(GetAllTrucks(), Request.Query,
                true, "trucksGrid", Trucks.Columns)
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .WithGridItemsCount()
                    .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            var items = server.ItemsToDisplay;
            return Ok(items);
        }

        private IEnumerable<Truck> GetAllTrucks()
        {
            var trucks = new List<Truck>();
            trucks.Add(new Truck
            {
                Id = 1,
                Description = "Truck 1",
                Person = new Person
                {
                    Id = 1,
                    FirstName = "Person",
                    LastName = "1"
                },
                Type = PersonType.DriverAndOwner
            });
            trucks.Add(new Truck
            {
                Id = 2,
                Description = "Truck 2",
                Person = new Person
                {
                    Id = 2,
                    LastName = "2"
                },
                Type = PersonType.Driver
            });
            trucks.Add(new Truck
            {
                Id = 3,
                Description = "Truck 3",
                Person = new Person
                {
                    Id = 1,
                    FirstName = "Person"
                },
                Type = PersonType.Owner
            });
            trucks.Add(new Truck
            {
                Id = 4,
                Description = "Truck 4",
                Person = new Person
                {
                    Id = 4,
                    FirstName = "Person",
                    LastName = "4"
                },
                Type = PersonType.Driver
            });
            trucks.Add(new Truck
            {
                Id = 5,
                Description = "Truck 5",
                Person = new Person
                {
                    Id = 5,
                    FirstName = "Person",
                    LastName = "5"
                },
                Type = PersonType.Driver
            });
            trucks.Add(new Truck
            {
                Id = 6,
                Description = "Truck 6",
                Person = new Person
                {
                    Id = 6,
                    FirstName = "Person",
                    LastName = "6"
                },
                Type = PersonType.Owner
            });
            return trucks;
        }
    }
}
