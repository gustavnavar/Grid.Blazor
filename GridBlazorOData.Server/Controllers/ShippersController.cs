using GridBlazorOData.Server.Models;
using GridBlazorOData.Shared.Models;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GridBlazorOData.Server.Controllers
{
    public class ShippersController : ODataController
    {
        private readonly NorthwindDbContext _context;

        public ShippersController(NorthwindDbContext context)
        {
            _context = context;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            var repository = new ShipperRepository(_context);
            var shippers = repository.GetAll();
            return Ok(shippers);
        }

        [EnableQuery]
        public async Task<IActionResult> Get(string key)
        {
            var repository = new ShipperRepository(_context);
            Shipper shipper = await repository.GetById(key);
            return Ok(shipper);
        }
    }
}
