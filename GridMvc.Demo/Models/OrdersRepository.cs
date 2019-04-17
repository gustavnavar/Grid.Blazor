using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GridMvc.Demo.Models
{
    public class OrdersRepository : SqlRepository<Order>
    {
        public OrdersRepository(NorthwindDbContext context)
            : base(context)
        {
        }

        public override IQueryable<Order> GetAll()
        {
            return EfDbSet.Include("Customer");
        }

        public override Order GetById(object id)
        {
            return GetAll().FirstOrDefault(o => o.OrderID == (int) id);
        }
    }
}