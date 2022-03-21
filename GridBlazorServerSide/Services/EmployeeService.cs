using GridBlazorServerSide.Data;
using GridBlazorServerSide.Models;
using GridCore.Server;
using GridShared;
using GridShared.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GridBlazorServerSide.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly DbContextOptions<NorthwindDbContext> _options;

        public EmployeeService(DbContextOptions<NorthwindDbContext> options)
        {
            _options = options;
        }

        public async Task<ItemsDTO<Employee>> GetEmployeesGridRowsAsync(Action<IGridColumnCollection<Employee>> columns,
            QueryDictionary<StringValues> query)
        {
            using (var context = new NorthwindDbContext(_options))
            {
                var repository = new EmployeeRepository(context);
                var server = new GridCoreServer<Employee>(repository.GetAll(), query, true, "employeesGrid", columns)
                        .Sortable()
                        .WithPaging(10)
                        .Filterable()
                        .WithMultipleFilters()
                        .Groupable(true)
                        .Searchable(true, false, false)
                        .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

                // return items to displays
                var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
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

        public async Task<Employee> Get(params object[] keys)
        {
            using (var context = new NorthwindDbContext(_options))
            {
                int employeeId;
                int.TryParse(keys[0].ToString(), out employeeId);
                var repository = new EmployeeRepository(context);
                return await repository.GetById(employeeId);
            }
        }

        public async Task Insert(Employee item)
        {
            using (var context = new NorthwindDbContext(_options))
            {
                try
                {
                    var repository = new EmployeeRepository(context);
                    await repository.Insert(item);
                    repository.Save();
                }
                catch (Exception e)
                {
                    throw new GridException("DETSRV-01", e);
                }
            }
        }

        public async Task Update(Employee item)
        {
            using (var context = new NorthwindDbContext(_options))
            {
                try
                {
                    var repository = new EmployeeRepository(context);
                    await repository.Update(item);
                    repository.Save();
                }
                catch (Exception e)
                {
                    throw new GridException(e);
                }
            }
        }

        public async Task Delete(params object[] keys)
        {
            using (var context = new NorthwindDbContext(_options))
            {
                try
                {
                    var employee = await Get(keys);
                    var repository = new EmployeeRepository(context);
                    repository.Delete(employee);
                    repository.Save();
                }
                catch (Exception)
                {
                    throw new GridException("Error deleting the employee");
                }
            }
        }
    }

    public interface IEmployeeService : ICrudDataService<Employee>
    {
        Task<ItemsDTO<Employee>> GetEmployeesGridRowsAsync(Action<IGridColumnCollection<Employee>> columns,
            QueryDictionary<StringValues> query);
        IEnumerable<SelectItem> GetAllEmployees();
    }
}
