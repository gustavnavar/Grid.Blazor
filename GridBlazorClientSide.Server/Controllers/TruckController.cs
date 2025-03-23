using GridBlazorClientSide.Client.Pages;
using GridBlazorClientSide.Server.Models;
using GridBlazorClientSide.Shared.Models;
using GridCore.Server;
using GridMvc.Server;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GridBlazorClientSide.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TruckController : Controller
    {

        public TruckController()
        {}

        [HttpGet("[action]")]
        public ActionResult GetTrucksGrid()
        {
            IGridServer<Truck> server = new GridServer<Truck>(GetAllTrucks(), Request.Query,
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

        [HttpGet("{id}")]
        public ActionResult GetTruck(int id)
        {
            Truck truck = GetAllTrucks().SingleOrDefault(r => r.Id == id);
            if (truck == null)
            {
                return NotFound();
            }
            return Ok(truck);
        }

        private IEnumerable<Truck> GetAllTrucks()
        {
            var trucks = new List<Truck>();
            trucks.Add(new Truck
            {
                Id = 1,
                Description = "Truck 1",
                Date = new DateOnly(2021, 1, 1),
                Time = new TimeOnly(12, 30),
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
                Date = new DateOnly(2021, 1, 4),
                Time = new TimeOnly(14, 30),
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
                Date = new DateOnly(2021, 1, 8),
                Time = new TimeOnly(16, 30),
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
                Date = new DateOnly(2021, 1, 12),
                Time = new TimeOnly(18, 30),
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
                Date = new DateOnly(2021, 1, 16),
                Time = new TimeOnly(20, 30),
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
                Date = new DateOnly(2021, 1, 20),
                Time = new TimeOnly(22, 30),
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
