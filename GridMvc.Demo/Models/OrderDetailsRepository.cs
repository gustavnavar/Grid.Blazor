using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GridMvc.Demo.Models
{
    public class OrderDetailsRepository : SqlRepository<OrderDetail>
    {
        public OrderDetailsRepository(NorthwindDbContext context)
            : base(context)
        {
        }

        public override IQueryable<OrderDetail> GetAll()
        {
            return EfDbSet.Include(r => r.Product);
        }

        public override async Task<OrderDetail> GetById(object id)
        {
            return await GetAll().SingleOrDefaultAsync(o => o.OrderID == (int)id);
        }

        public IEnumerable<OrderDetail> GetForOrder(int id)
        {
            return  GetAll().Where(o => o.OrderID == id).ToList();
        }
    }
}