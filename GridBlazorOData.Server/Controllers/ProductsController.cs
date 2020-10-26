using GridBlazorOData.Server.Models;
using GridBlazorOData.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System.Threading.Tasks;

namespace GridBlazorOData.Server.Controllers
{
    public class ProductsController : ODataController
    {
        private readonly NorthwindDbContext _context;

        public ProductsController(NorthwindDbContext context)
        {
            _context = context;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            var repository = new ProductRepository(_context);
            var products = repository.GetAll();
            return Ok(products);
        }

        [EnableQuery]
        public async Task<IActionResult> Get(string key)
        {
            var repository = new ProductRepository(_context);
            Product product = await repository.GetById(key);
            return Ok(product);
        }
    }
}
