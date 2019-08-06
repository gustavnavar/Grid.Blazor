using GridBlazorServerSide.Data;
using GridBlazorServerSide.Models;
using GridMvc.Server;
using GridShared;
using GridShared.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GridBlazorServerSide.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly NorthwindDbContext _context;

        public CustomerService()
        {
            var builder = new DbContextOptionsBuilder<NorthwindDbContext>();
            builder.UseSqlServer(Startup.ConnectionString);
            _context = new NorthwindDbContext(builder.Options);
        }

        public IEnumerable<string> GetCustomersNames()
        {
            var repository = new CustomersRepository(_context);
            return repository.GetAll().Select(r => r.CompanyName).ToList();
        }

        public ItemsDTO<Customer> GetCustomersGridRows(Action<IGridColumnCollection<Customer>> columns,
            QueryDictionary<StringValues> query)
        {
            var repository = new CustomersRepository(_context);
            var server = new GridServer<Customer>(repository.GetAll(), new QueryCollection(query),
                true, "customersGrid", columns)
                    .Sortable()
                    .WithPaging(10)
                    .Filterable()
                    .WithMultipleFilters()
                    .Searchable(true, false);

            // return items to displays
            var items = server.ItemsToDisplay;
            return items;
        }
    }

    public interface ICustomerService
    {
        IEnumerable<string> GetCustomersNames();
        ItemsDTO<Customer> GetCustomersGridRows(Action<IGridColumnCollection<Customer>> columns, QueryDictionary<StringValues> query);
    }
}
