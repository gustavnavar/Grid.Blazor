using GridShared;
using GridShared.Utility;
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
            return Ok(GetOrdersGridRows(GridSample.Columns));
        }

        private ItemsDTO<Order> GetOrdersGridRows(Action<IGridColumnCollection<Order>> columns)
        {
            var repository = new OrdersRepository(_context);
            var server = new GridServer<Order>(repository.GetAll(), Request.Query,
                true, "ordersGrid", columns, 10)
                    .Sortable()
                    .Filterable()
                    .WithMultipleFilters();

            // return items to displays
            return server.ItemsToDisplay;
        }

        [HttpGet("[action]")]
        public ActionResult GetCustomersNames()
        {
            var repository = new CustomersRepository(_context);
            return Ok(repository.GetAll().Select(r => r.CompanyName));
        }
    }
}
