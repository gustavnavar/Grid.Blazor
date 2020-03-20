using GridMvc.Server;
using GridShared;
using GridShared.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GridBlazorServerSide.Services
{
    public class TruckService : ITruckService
    {
        public ItemsDTO<Truck> GetTrucksGridRows(Action<IGridColumnCollection<Truck>> columns,
            QueryDictionary<StringValues> query)
        {
            var server = new GridServer<Truck>(GetAll(), new QueryCollection(query),
                    true, "trucksGrid", columns)
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
                truck_id = 1,
                description = "Truck 1",
                owner_person = new Person {
                    person_id = 1,
                    first_name = "Owner",
                    last_name = "1"
                }
            });
            trucks.Add(new Truck
            {
                truck_id = 2,
                description = "Truck 2",
                owner_person = new Person
                {
                    person_id = 2,
                    last_name = "2"
                }
            });
            trucks.Add(new Truck
            {
                truck_id = 3,
                description = "Truck 3",
                owner_person = new Person
                {
                    person_id = 1,
                    first_name = "Owner"
                }
            });
            trucks.Add(new Truck
            {
                truck_id = 4,
                description = "Truck 4",
                owner_person = new Person
                {
                    first_name = "Owner",
                    last_name = "4"
                }
            });
            trucks.Add(new Truck
            {
                truck_id = 5,
                description = "Truck 5"
            });
            trucks.Add(new Truck
            {
                description = "Truck 6",
                owner_person = new Person
                {
                    person_id = 6,
                    first_name = "Owner",
                    last_name = "6"
                }
            });
            return trucks;
        }
    }

    [Serializable]
    public class Truck
    {
        [Key]
        public int? truck_id { get; set; }
        public string description { get; set; }
        public Person owner_person { get; set; }
    }

    [Serializable]
    public class Person
    {
        [Key]
        public int? person_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string full_name => $"{ first_name } { last_name }";
    }

    public interface ITruckService
    {
        ItemsDTO<Truck> GetTrucksGridRows(Action<IGridColumnCollection<Truck>> columns,
            QueryDictionary<StringValues> query);
    }
}
