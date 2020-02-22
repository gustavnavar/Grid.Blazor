using GridBlazorClientSide.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GridBlazorClientSide.Server.Models
{
    public class ProductRepository : SqlRepository<Product>
    {
        public ProductRepository(NorthwindDbContext context)
            : base(context)
        {
        }

        public override IQueryable<Product> GetAll()
        {
            return EfDbSet;
        }

        public override async Task<Product> GetById(object id)
        {
            return await GetAll().SingleOrDefaultAsync(c => c.ProductID == (int)id);
        }
    }
}
