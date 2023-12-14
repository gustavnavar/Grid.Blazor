using GridBlazorServerSide.Data;
using GridBlazorServerSide.Models;
using GridCore;
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

        public IEnumerable<string> GetCustomersNames(Action<IGridColumnCollection<Order>> columns, 
            QueryDictionary<string> query)
        {
            using (var context = new NorthwindDbContext(_options))
            {
                // get all customer ids in the grid with the current filters
                var orderRepository = new OrdersRepository(context);
                var server = new GridCoreServer<Order>(orderRepository.GetAll(), query, true, "ordersGrid", columns);
                return ((GridBase<Order>)server.Grid).GridItems.Where(r => !string.IsNullOrWhiteSpace(r.CustomerID))
                    .Select(r => r.Customer).Distinct()
                    .Select(r => r.CompanyName)
                    .ToList();
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

        public IEnumerable<SelectItem> GetAllCustomers2()
        {
            using (var context = new NorthwindDbContext(_options))
            {
                CustomersRepository repository = new CustomersRepository(context);
                return repository.GetAll()
                    .Select(r => new SelectItem(r.CompanyName, r.CompanyName))
                    .ToList();
            }
        }

        public IEnumerable<SelectItem> GetAllContacts()
        {
            using (var context = new NorthwindDbContext(_options))
            {
                CustomersRepository repository = new CustomersRepository(context);
                return repository.GetAll()
                    .Select(r => new SelectItem(r.ContactName, r.ContactName))
                    .ToList();
            }
        }

        public async Task<ItemsDTO<Customer>> GetCustomersGridRowsAsync(Action<IGridColumnCollection<Customer>> columns,
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
                        .Searchable(true, false)
                        .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

                // return items to displays
                var items = await server.GetItemsToDisplayAsync(async x => await x.ToListAsync());
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
        IEnumerable<string> GetCustomersNames(Action<IGridColumnCollection<Order>> columns, QueryDictionary<string> query);
        IEnumerable<SelectItem> GetAllCustomers();
        IEnumerable<SelectItem> GetAllCustomers2();
        IEnumerable<SelectItem> GetAllContacts();
        Task<ItemsDTO<Customer>> GetCustomersGridRowsAsync(Action<IGridColumnCollection<Customer>> columns,
            QueryDictionary<StringValues> query);
    }
}
