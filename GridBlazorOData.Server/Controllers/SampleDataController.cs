using GridBlazorOData.Server.Models;
using GridBlazorOData.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GridBlazorOData.Server.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        private readonly NorthwindDbContext _context;
        
        public SampleDataController(NorthwindDbContext context)
        {
            _context = context;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> SetEmployeePhoto([FromBody] Employee employee)
        {
            if (ModelState.IsValid)
            {
                var repository = new EmployeeRepository(_context);
                try
                {
                    await repository.UpdatePhoto(employee.EmployeeID, employee.Base64String);
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
    }
}
