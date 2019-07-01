using GridBlazor.Demo.Client.Pages;
using GridBlazor.Demo.Server.Models;
using GridBlazor.Demo.Shared.Models;
using GridMvc.Server;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GridBlazor.Demo.Server.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        private static string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly NorthwindDbContext _context;

        public SampleDataController(NorthwindDbContext context)
        {
            _context = context;
        }

        [HttpGet("[action]")]
        public IEnumerable<WeatherForecast> WeatherForecasts()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });
        }

        [HttpGet("[action]")]
        public ActionResult GetOrdersGridForSample()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", GridSample.OrderColumns)
                    .WithPaging(10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters()
                    .Searchable(true, false);

            var items = server.ItemsToDisplay;

            // uncomment the following lines are to test null responses
            //items = null;
            //items.Items = null;
            //items.Pager = null;
            return Ok(items);
        }

        [HttpGet("[action]")]
        public ActionResult GetOrdersAutoGenerateColumns()
        {
            var repository = new OrdersRepository(_context);
            IGridServer<Order> server = new GridServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", null)
                    .AutoGenerateColumns();

            return Ok(server.ItemsToDisplay);
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
                    false, "orderDetailsGrid" + OrderId.ToString(), GridSample.OrderDetailColumns)
                        .WithPaging(10)
                        .Sortable()
                        .Filterable()
                        .WithMultipleFilters();

            var items = server.ItemsToDisplay;
            return Ok(items);
        }
    }
}
