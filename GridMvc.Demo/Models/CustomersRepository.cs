using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GridMvc.Demo.Models
{
    public class CustomersRepository : SqlRepository<Customer>
    {
        public CustomersRepository(NorthwindDbContext context)
            : base(context)
        {
        }

        public override IQueryable<Customer> GetAll()
        {
            return EfDbSet;
        }

        public override async Task<Customer> GetById(object id)
        {
            return await GetAll().SingleOrDefaultAsync(c => c.CustomerID == (string)id);
        }
    }
}