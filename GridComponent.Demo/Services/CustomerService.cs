using GridComponent.Demo.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace GridComponent.Demo.Services
{
    public class CustomerService
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
    }
}
