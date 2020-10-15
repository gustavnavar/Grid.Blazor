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
    public class CustomerController : Controller
    {
        private readonly NorthwindDbContext _context;

        public CustomerController(NorthwindDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult GetAllCustomers()
        {
            var repository = new CustomersRepository(_context);
            var customers = repository.GetAll().ToList();
            if (customers == null)
            {
                return NotFound();
            }
            return Ok(customers);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Customer customer)
        {
            if (ModelState.IsValid)
            {
                if (customer == null)
                {
                    return BadRequest();
                }

                var repository = new CustomersRepository(_context);
                try
                {
                    await repository.Insert(customer);
                    repository.Save();

                    return Ok();
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

        [HttpGet("{id}")]
        public async Task<ActionResult> GetCustomer(string id)
        {
            var repository = new CustomersRepository(_context);
            Customer customer = await repository.GetById(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCustomer(string id, [FromBody] Customer customer)
        {
            if (ModelState.IsValid)
            {
                if (customer == null || customer.CustomerID != id)
                {
                    return BadRequest();
                }

                var repository = new CustomersRepository(_context);
                try
                {
                    await repository.Update(customer);
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

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var repository = new CustomersRepository(_context);
            Customer customer = await repository.GetById(id);

            if (customer == null)
            {
                return NotFound();
            }

            try
            {
                repository.Delete(customer);
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
