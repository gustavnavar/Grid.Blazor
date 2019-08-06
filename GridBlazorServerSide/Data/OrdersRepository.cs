using GridBlazorServerSide.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GridBlazorServerSide.Data
{
    public class OrdersRepository : SqlRepository<Order>, IOrdersRepository
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

        public void Update(Order order)
        {
            var entry = Context.Entry(order);
            if (entry.State == EntityState.Detached)
            {
                var attachedOrder = GetById(order.OrderID);
                if (attachedOrder != null)
                {
                    Context.Entry(attachedOrder).CurrentValues.SetValues(order);
                }
                else
                {
                    entry.State = EntityState.Modified;
                }
            }
        }

        public void Save()
        {
            Context.SaveChanges();
        }
    }

    public interface IOrdersRepository
    {
        void Update(Order order);
        void Save();
    }
}