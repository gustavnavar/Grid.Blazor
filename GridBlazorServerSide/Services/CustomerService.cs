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
                var server = new GridCoreServer<Customer>(repository.GetAll(), query, true, "customersGrid", columns)
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

        public async Task<Customer> Get(params object[] keys)
        {
            using (var context = new NorthwindDbContext(_options))
            {
                var repository = new CustomersRepository(context);
                return await repository.GetById(keys[0].ToString());
            }
        }

        public async Task Insert(Customer item)
        {
            using (var context = new NorthwindDbContext(_options))
            {
                try
                {
                    var repository = new CustomersRepository(context);
                    await repository.Insert(item);
                    repository.Save();
                }
                catch (Exception e)
                {
                    throw new GridException("CUSSRV-01", e);
                }
            }
        }

        public async Task Update(Customer item)
        {
            using (var context = new NorthwindDbContext(_options))
            {
                try
                {
                    var repository = new CustomersRepository(context);
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
                    var customer = await Get(keys);
                    var repository = new CustomersRepository(context);
                    repository.Delete(customer);
                    repository.Save();
                }
                catch (Exception)
                {
                    throw new GridException("Error deleting the customer");
                }
            }
        }
    }

    public interface ICustomerService : ICrudDataService<Customer>
    {
        IEnumerable<string> GetCustomersNames();
        IEnumerable<SelectItem> GetAllCustomers();
        ItemsDTO<Customer> GetCustomersGridRows(Action<IGridColumnCollection<Customer>> columns, QueryDictionary<StringValues> query);
    }
}
