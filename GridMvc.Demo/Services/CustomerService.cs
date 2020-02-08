using GridMvc.Demo.Models;
using GridShared;
using Microsoft.EntityFrameworkCore;
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
    }

    public interface ICustomerService
    {
        IEnumerable<string> GetCustomersNames();
        IEnumerable<SelectItem> GetAllCustomers();
    }
}
