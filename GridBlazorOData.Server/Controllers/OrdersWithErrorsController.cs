using GridBlazorOData.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System;

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
