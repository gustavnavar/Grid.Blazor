using GridBlazorServerSide.Models;
using GridCore.Server;
using GridShared;
using GridShared.Utility;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GridBlazorServerSide.Services
{
    public class TruckService : ITruckService
    {
        public ItemsDTO<Truck> GetTrucksGridRows(Action<IGridColumnCollection<Truck>> columns,
            QueryDictionary<StringValues> query)
        {
            var server = new GridCoreServer<Truck>(GetAll(), query, true, "trucksGrid", columns)
                        .Sortable()
                        .WithPaging(10)
                        .Filterable()
                        .WithMultipleFilters()
                        .Groupable(true)
                        .Searchable(true, false, false);

            // return items to displays
            var items = server.ItemsToDisplay;
            return items;
        }

        private IEnumerable<Truck> GetAll()
        {
            var trucks = new List<Truck>();
            trucks.Add(new Truck {
                Id = 1,
                Description = "Truck 1",
                Person = new Person {
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

        public IEnumerable<SelectItem> GetTypes()
        {
            return new PersonType[] { PersonType.Driver, PersonType.Owner, PersonType.DriverAndOwner }
                .Select(r => new SelectItem(r.ToString(), r.ToText()));
        }
    }

    public interface ITruckService
    {
        ItemsDTO<Truck> GetTrucksGridRows(Action<IGridColumnCollection<Truck>> columns,
            QueryDictionary<StringValues> query);
        IEnumerable<SelectItem> GetTypes();
    }
}
