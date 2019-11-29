using GridBlazorServerSide.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GridBlazorServerSide.Data
{
    public class EmployeeRepository : SqlRepository<Employee>
    {
        public EmployeeRepository(NorthwindDbContext context)
            : base(context)
        {
        }

        public override IQueryable<Employee> GetAll()
        {
            return EfDbSet;
        }

        public override async Task<Employee> GetById(object id)
        {
            return await GetAll().SingleOrDefaultAsync(c => c.EmployeeID == (int)id);
        }
    }
}