using GridBlazorOData.Server.Models;
using GridBlazorOData.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GridBlazorOData.Server.Controllers
{
    public class OrderDetailsController : ODataController
    {
        private readonly NorthwindDbContext _context;

        public OrderDetailsController(NorthwindDbContext context)
        {
            _context = context;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            var repository = new OrderDetailsRepository(_context);
            var orderDetails = repository.GetAll();
            return Ok(orderDetails);
        }

        [EnableQuery]
        public async Task<IActionResult> Get([FromODataUri] int keyOrderID, [FromODataUri] int keyProductID)
        {
            var repository = new OrderDetailsRepository(_context);
            OrderDetail orderDetail = await repository.GetById(new { OrderID = keyOrderID, ProductID = keyProductID });
            return Ok(orderDetail);
        }

        public async Task<IActionResult> Post([FromBody]OrderDetail orderdetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (orderdetail == null)
            {
                return BadRequest();
            }

            var repository = new OrderDetailsRepository(_context);
            try
            {
                await repository.Insert(orderdetail);
                repository.Save();

                return Created(orderdetail);
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    message = e.Message.Replace('{', '(').Replace('}', ')')
                });
            }
        }

        public async Task<IActionResult> Patch([FromODataUri] int keyOrderID, [FromODataUri] int keyProductID, 
            [FromBody] Delta<OrderDetail> orderDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var repository = new OrderDetailsRepository(_context);
            OrderDetail entity = await repository.GetById(new { OrderID = keyOrderID, ProductID = keyProductID });
            if (entity == null)
            {
                return NotFound();
            }
            orderDetail.Patch(entity);
            try
            {
                repository.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderDetailExists(keyOrderID, keyProductID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(entity);
        }

        public async Task<IActionResult> Put([FromODataUri] int keyOrderID, [FromODataUri] int keyProductID, 
            [FromBody] OrderDetail orderDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (keyOrderID != orderDetail.OrderID || keyProductID != orderDetail.ProductID)
            {
                return BadRequest();
            }

            var repository = new OrderDetailsRepository(_context);
            await repository.Update(orderDetail);
            try
            {
                repository.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderDetailExists(keyOrderID, keyProductID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Updated(orderDetail);
        }

        public async Task<ActionResult> Delete([FromODataUri] int keyOrderID, [FromODataUri] int keyProductID)
        {
            var repository = new OrderDetailsRepository(_context);
            OrderDetail orderDetail = await repository.GetById(new { OrderID = keyOrderID, ProductID = keyProductID });
            if (orderDetail == null)
            {
                return NotFound();
            }
            repository.Delete(orderDetail);
            repository.Save();
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        private bool OrderDetailExists(int orderID, int productID)
        {
            var repository = new OrderDetailsRepository(_context);
            return repository.GetAll().Any(x => x.OrderID == orderID && x.ProductID == productID);
        }
    }
}
