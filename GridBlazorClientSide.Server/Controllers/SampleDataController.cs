using GridBlazorClientSide.Client.ColumnCollections;
using GridBlazorClientSide.Server.Models;
using GridBlazorClientSide.Shared.Models;
using GridMvc.Server;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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
                    .Searchable(true, false);

            var items = server.ItemsToDisplay;
            return Ok(items);
        }

        [HttpGet("[action]")]
        public ActionResult GetOrdersGridGroupable()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", ColumnCollections.OrderColumns)
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
                true, "ordersGrid", c => ColumnCollections.OrderColumnsWithEdit(c,null))
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
                true, "ordersGrid", ColumnCollections.OrderColumnsWithCrud)
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
                true, "ordersGrid", c => ColumnCollections.OrderColumnsAllFeatures(c, null))
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
        public ActionResult GetOrderDetailsGridAllFeatures(int OrderId)
        {
            var orderDetails = (new OrderDetailsRepository(_context)).GetForOrder(OrderId);

            var server = new GridServer<OrderDetail>(orderDetails, Request.Query,
                    false, "orderDetailsGrid" + OrderId.ToString(), ColumnCollections.OrderDetailColumnsAllFeatures)
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
    }
}
