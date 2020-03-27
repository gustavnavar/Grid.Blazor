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
    public class EmployeeService : IEmployeeService
    {
        private readonly DbContextOptions<NorthwindDbContext> _options;

        public EmployeeService(DbContextOptions<NorthwindDbContext> options)
        {
            _options = options;
        }

        public ItemsDTO<Employee> GetEmployeesGridRows(Action<IGridColumnCollection<Employee>> columns,
            QueryDictionary<StringValues> query)
        {
            using (var context = new NorthwindDbContext(_options))
            {
                var repository = new EmployeeRepository(context);
                var server = new GridServer<Employee>(repository.GetAll(), new QueryCollection(query),
                    true, "employeesGrid", columns)
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
        }

        public IEnumerable<SelectItem> GetAllEmployees()
        {
            using (var context = new NorthwindDbContext(_options))
            {
                EmployeeRepository repository = new EmployeeRepository(context);
                return repository.GetAll()
                    .Select(r => new SelectItem(r.EmployeeID.ToString(), r.EmployeeID.ToString() + " - " 
                        + r.FirstName + " " + r.LastName))
                    .ToList();
            }
        }
    }

    public interface IEmployeeService
    {
        ItemsDTO<Employee> GetEmployeesGridRows(Action<IGridColumnCollection<Employee>> columns,
            QueryDictionary<StringValues> query);
        IEnumerable<SelectItem> GetAllEmployees();
    }
}
