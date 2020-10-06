using GridBlazorOData.Server.Models;
using GridBlazorOData.Shared.Models;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GridBlazorOData.Server.Controllers
{
    public class OrdersWithErrorsController : ODataController
    {
        private readonly NorthwindDbContext _context;

        public OrdersWithErrorsController(NorthwindDbContext context)
        {
            _context = context;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            var random = new Random();
            if (random.Next(2) == 0)
                return BadRequest();

            var repository = new OrdersRepository(_context);
            var orders = repository.GetAll();
            return Ok(orders);
        }
    }
}
