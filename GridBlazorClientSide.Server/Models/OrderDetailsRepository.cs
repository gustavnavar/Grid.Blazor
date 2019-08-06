using GridBlazorClientSide.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace GridBlazorClientSide.Server.Models
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

        public override OrderDetail GetById(object id)
        {
            return GetAll().FirstOrDefault(o => o.OrderID == (int)id);
        }

        public IEnumerable<OrderDetail> GetForOrder(int id)
        {
            return GetAll().Where(o => o.OrderID == id);
        }
    }
}
