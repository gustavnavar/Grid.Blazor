using GridBlazorClientSide.Server.Models;
using GridBlazorClientSide.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GridBlazorClientSide.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController : Controller
    {
        private readonly NorthwindDbContext _context;

        public OrderDetailController(NorthwindDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult GetAllOrderDetails()
        {
            var repository = new OrderDetailsRepository(_context);
            var orders = repository.GetAll().ToList();
            if (orders == null)
            {
                return NotFound();
            }
            return Ok(orders);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                if (orderDetail == null)
                {
                    return BadRequest();
                }

                var repository = new OrderDetailsRepository(_context);
                try
                {
                    await repository.Insert(orderDetail);
                    repository.Save();

                    return NoContent();
                }
                catch (Exception e)
                {
                    return BadRequest(new
                    {
                        message = e.Message.Replace('{', '(').Replace('}', ')')
                    });
                }
            }
            return BadRequest(new
            {
                message = "ModelState is not valid"
            });
        }

        [HttpGet("{orderId}/{productId}")]
        public async Task<ActionResult> GetOrderDetail(int orderId, int productId)
        {
            var repository = new OrderDetailsRepository(_context);
            OrderDetail orderDetail = await repository.GetById(new { OrderID = orderId, ProductID = productId });
            if (orderDetail == null)
            {
                return NotFound();
            }
            return Ok(orderDetail);
        }

        [HttpPut("{orderId}/{productId}")]
        public async Task<ActionResult> UpdateOrderDetail(int orderId, int productId, [FromBody] OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                if (orderDetail == null || orderDetail.OrderID != orderId  || orderDetail.ProductID != productId)
                {
                    return BadRequest();
                }

                var repository = new OrderDetailsRepository(_context);
                try
                {
                    await repository.Update(orderDetail);
                    repository.Save();

                    return NoContent();
                }
                catch (Exception e)
                {
                    return BadRequest(new
                    {
                        message = e.Message.Replace('{', '(').Replace('}', ')')
                    });
                }
            }
            return BadRequest(new
            {
                message = "ModelState is not valid"
            });
        }

        [HttpDelete("{orderId}/{productId}")]
        public async Task<ActionResult> Delete(int orderId, int productId)
        {
            var repository = new OrderDetailsRepository(_context);
            OrderDetail orderDetail = await repository.GetById(new { OrderID = orderId, ProductID = productId });

            if (orderDetail == null)
            {
                return NotFound();
            }

            try
            {
                repository.Delete(orderDetail);
                repository.Save();

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    message = e.Message.Replace('{', '(').Replace('}', ')')
                });
            }
        }
    }
}
