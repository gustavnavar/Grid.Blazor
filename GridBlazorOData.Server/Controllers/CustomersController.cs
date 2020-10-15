using GridBlazorOData.Server.Models;
using GridBlazorOData.Shared.Models;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GridBlazorOData.Server.Controllers
{
    public class CustomersController : ODataController
    {
        private readonly NorthwindDbContext _context;

        public CustomersController(NorthwindDbContext context)
        {
            _context = context;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            var repository = new CustomersRepository(_context);
            var customers = repository.GetAll();
            return Ok(customers);
        }

        [EnableQuery]
        public async Task<IActionResult> Get(string key)
        {
            var repository = new CustomersRepository(_context);
            Customer customer = await repository.GetById(key);
            return Ok(customer);
        }
    }
}
