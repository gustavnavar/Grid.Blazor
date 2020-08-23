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
    public class EmployeeController : Controller
    {
        private readonly NorthwindDbContext _context;

        public EmployeeController(NorthwindDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult GetAllEmployees()
        {
            var repository = new EmployeeRepository(_context);
            var employees = repository.GetAll().ToList();
            if (employees == null)
            {
                return NotFound();
            }
            return Ok(employees);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Employee employee)
        {
            if (ModelState.IsValid)
            {
                if (employee == null)
                {
                    return BadRequest();
                }

                var repository = new EmployeeRepository(_context);
                try
                {
                    await repository.Insert(employee);
                    repository.Save();

                    return Ok(employee.EmployeeID);
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
        public async Task<ActionResult> GetEmployee(int id)
        {
            var repository = new EmployeeRepository(_context);
            Employee employee = await repository.GetById(id);
            if (employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateEmployee(int id, [FromBody] Employee employee)
        {
            if (ModelState.IsValid)
            {
                if (employee == null || employee.EmployeeID != id)
                {
                    return BadRequest();
                }

                var repository = new EmployeeRepository(_context);
                try
                {
                    await repository.Update(employee);
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
        public async Task<ActionResult> Delete(int id)
        {
            var repository = new EmployeeRepository(_context);
            Employee employee = await repository.GetById(id);

            if (employee == null)
            {
                return NotFound();
            }

            try
            {
                repository.Delete(employee);
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
