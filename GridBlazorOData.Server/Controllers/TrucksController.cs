using GridBlazorOData.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System.Collections.Generic;
using System.Linq;

namespace GridBlazorOData.Server.Controllers
{
    public class TrucksController : ODataController
    {
        public TrucksController()
        {
        }

        [EnableQuery]
        public IActionResult Get()
        {
            var trucks = GetAll();
            return Ok(trucks);
        }

        [EnableQuery]
        public IActionResult Get(int key)
        {
            Truck truck = GetAll().SingleOrDefault(r => r.Id == key);
            return Ok(truck);
        }

        private IEnumerable<Truck> GetAll()
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
