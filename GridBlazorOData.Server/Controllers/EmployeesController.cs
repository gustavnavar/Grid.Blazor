using GridBlazorOData.Server.Models;
using GridBlazorOData.Shared.Models;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GridBlazorOData.Server.Controllers
{
    public class EmployeesController : ODataController
    {
        private readonly NorthwindDbContext _context;

        public EmployeesController(NorthwindDbContext context)
        {
            _context = context;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            var repository = new EmployeeRepository(_context);
            var employees = repository.GetAll();
            return Ok(employees);
        }

        [EnableQuery]
        public async Task<IActionResult> Get(string key)
        {
            var repository = new EmployeeRepository(_context);
            Employee employee = await repository.GetById(key);
            return Ok(employee);
        }
    }
}
