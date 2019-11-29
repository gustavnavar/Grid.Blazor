using GridBlazorServerSide.Data;
using GridShared;
using Microsoft.EntityFrameworkCore;
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
        IEnumerable<SelectItem> GetAllEmployees();
    }
}
