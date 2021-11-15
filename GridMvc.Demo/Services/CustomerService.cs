using GridMvc.Demo.Models;
using GridMvc.Server;
using GridShared;
using GridShared.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GridMvc.Demo.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly DbContextOptions<NorthwindDbContext> _options;

        public CustomerService(DbContextOptions<NorthwindDbContext> options)
        {
            _options = options;
        }

        public IEnumerable<string> GetCustomersNames()
        {
            using (var context = new NorthwindDbContext(_options))
            {
                var repository = new CustomersRepository(context);
                return repository.GetAll().Select(r => r.CompanyName).ToList();
            }
        }

        public IEnumerable<SelectItem> GetAllCustomers()
        {
            using (var context = new NorthwindDbContext(_options))
            {
                CustomersRepository repository = new CustomersRepository(context);
                return repository.GetAll()
                    .Select(r => new SelectItem(r.CustomerID, r.CustomerID + " - " + r.CompanyName))
                    .ToList();
            }
        }

        public ItemsDTO<Customer> GetCustomersGridRows(Action<IGridColumnCollection<Customer>> columns,
            QueryDictionary<StringValues> query)
        {
            using (var context = new NorthwindDbContext(_options))
            {
                var repository = new CustomersRepository(context);
                var server = new GridServer<Customer>(repository.GetAll(), new QueryCollection(query),
                    true, "customersGrid", columns)
                        .Sortable()
                        .WithPaging(10)
                        .Filterable()
                        .WithMultipleFilters()
                        .Searchable(true, false)
                        .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

                // return items to displays
                var items = server.ItemsToDisplay;
                return items;
            }
        }
    }

    public interface ICustomerService
    {
        IEnumerable<string> GetCustomersNames();
        IEnumerable<SelectItem> GetAllCustomers();
        ItemsDTO<Customer> GetCustomersGridRows(Action<IGridColumnCollection<Customer>> columns, QueryDictionary<StringValues> query);
    }
}
