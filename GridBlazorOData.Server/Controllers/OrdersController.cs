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
    public class OrdersController : ODataController
    {
        private readonly NorthwindDbContext _context;

        public OrdersController(NorthwindDbContext context)
        {
            _context = context;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            var repository = new OrdersRepository(_context);
            var orders = repository.GetAll();
            return Ok(orders);
        }

        [EnableQuery]
        public async Task<IActionResult> Get(int key)
        {
            var repository = new OrdersRepository(_context);
            Order order = await repository.GetById(key);
            return Ok(order);
        }

        public async Task<IActionResult> Post([FromBody]Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }         

            if (order == null)
            {
                return BadRequest();
            }

            var repository = new OrdersRepository(_context);
            try
            {
                await repository.Insert(order);
                repository.Save();

                return Created(order);
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    message = e.Message.Replace('{', '(').Replace('}', ')')
                });
            }
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<Order> order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var repository = new OrdersRepository(_context);
            Order entity = await repository.GetById(key);
            if (entity == null)
            {
                return NotFound();
            }
            order.Patch(entity);
            try
            {
                repository.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(key))
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

        public async Task<IActionResult> Put([FromODataUri]int key, [FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (key != order.OrderID)
            {
                return BadRequest();
            }

            var repository = new OrdersRepository(_context);
            await repository.Update(order);
            try
            {              
                repository.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Updated(order);
        }

        public async Task<ActionResult> Delete([FromODataUri] int key)
        {
            var repository = new OrdersRepository(_context);
            Order order = await repository.GetById(key);
            if (order == null)
            {
                return NotFound();
            }
            repository.Delete(order);
            repository.Save();
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        private bool OrderExists(int key)
        {
            var repository = new OrdersRepository(_context);
            return repository.GetAll().Any(x => x.OrderID == key);
        }

        [EnableQuery]
        [HttpGet("odata/Orders({keyOrderID})/OrderDetails")]
        public IActionResult GetOrderDetails(int keyOrderID)
        {
            var repository = new OrderDetailsRepository(_context);
            var orderDetails = repository.GetAll().Where(r => r.OrderID == keyOrderID);
            return Ok(orderDetails);
        }

        [EnableQuery]
        [HttpGet("odata/Orders({keyOrderID})/OrderDetails(orderID={orderID},productID={productID})")]
        public async Task<IActionResult> GetOrderDetails([FromODataUri] int orderID, [FromODataUri] int productID)
        {
            var repository = new OrderDetailsRepository(_context);
            OrderDetail orderDetail = await repository.GetById(new { OrderID = orderID, ProductID = productID });
            return Ok(orderDetail);
        }

        [HttpPost("odata/Orders({keyOrderID})/OrderDetails")]
        public async Task<IActionResult> PostToOrderDetails([FromODataUri] int keyOrderID, [FromBody] OrderDetail orderdetail)
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

        [HttpPatch("odata/Orders({keyOrderID})/OrderDetails(orderID={orderID},productID={productID})")]
        public async Task<IActionResult> PatchToOrderDetails([FromODataUri] int orderID, [FromODataUri] int productID,
            [FromBody] Delta<OrderDetail> orderDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var repository = new OrderDetailsRepository(_context);
            OrderDetail entity = await repository.GetById(new { OrderID = orderID, ProductID = productID });
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
                if (!OrderDetailExists(orderID, productID))
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

        [HttpPut("odata/Orders({keyOrderID})/OrderDetails(orderID={orderID},productID={productID})")]
        public async Task<IActionResult> PutToOrderDetails([FromODataUri] int orderID, [FromODataUri] int productID,
            [FromBody] OrderDetail orderDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (orderID != orderDetail.OrderID || productID != orderDetail.ProductID)
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
                if (!OrderDetailExists(orderID, productID))
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

        [HttpDelete("odata/Orders({keyOrderID})/OrderDetails(orderID={orderID},productID={productID})")]
        public async Task<ActionResult> DeleteToOrderDetails([FromODataUri] int orderID, [FromODataUri] int productID)
        {
            var repository = new OrderDetailsRepository(_context);
            OrderDetail orderDetail = await repository.GetById(new { OrderID = orderID, ProductID = productID });
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
