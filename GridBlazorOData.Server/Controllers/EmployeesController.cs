using GridBlazorOData.Server.Models;
using GridBlazorOData.Shared.Models;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GridBlazorOData.Server.Controllers
{
    public class EmployeesController : ODataController
    {
        private readonly NorthwindDbContext _context;

        public EmployeesController(NorthwindDbContext context)
        {
            _context = context;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            var repository = new EmployeeRepository(_context);
            var employees = repository.GetAll();
            return Ok(employees);
        }

        [EnableQuery]
        public async Task<IActionResult> Get(int key)
        {
            var repository = new EmployeeRepository(_context);
            Employee employee = await repository.GetById(key);
            return Ok(employee);
        }


        public async Task<IActionResult> Post([FromBody] Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (employee == null)
            {
                return BadRequest();
            }

            var repository = new EmployeeRepository(_context);
            try
            {
                await repository.Insert(employee);
                repository.Save();

                return Created(employee);
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    message = e.Message.Replace('{', '(').Replace('}', ')')
                });
            }
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<Employee> employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var repository = new EmployeeRepository(_context);
            Employee entity = await repository.GetById(key);
            if (entity == null)
            {
                return NotFound();
            }
            employee.Patch(entity);
            try
            {
                repository.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(key))
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

        public async Task<IActionResult> Put([FromODataUri] int key, [FromBody] Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (key != employee.EmployeeID)
            {
                return BadRequest();
            }

            var repository = new EmployeeRepository(_context);
            await repository.Update(employee);
            try
            {
                repository.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Updated(employee);
        }

        public async Task<ActionResult> Delete([FromODataUri] int key)
        {
            var repository = new EmployeeRepository(_context);
            Employee employee = await repository.GetById(key);
            if (employee == null)
            {
                return NotFound();
            }
            repository.Delete(employee);
            repository.Save();
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        private bool EmployeeExists(int key)
        {
            var repository = new EmployeeRepository(_context);
            return repository.GetAll().Any(x => x.EmployeeID == key);
        }
    }
}