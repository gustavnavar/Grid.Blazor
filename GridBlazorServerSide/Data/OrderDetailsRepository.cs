using GridBlazorServerSide.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GridBlazorServerSide.Data
{
    public class OrderDetailsRepository : SqlRepository<OrderDetail>, IOrderDetailsRepository
    {
        public OrderDetailsRepository(NorthwindDbContext context)
            : base(context)
        {
        }

        public override IQueryable<OrderDetail> GetAll()
        {
            return EfDbSet.Include(r => r.Product);
        }

        public override async Task<OrderDetail> GetById(object ids)
        {
            var orderID = ids.GetType().GetProperty("OrderID");
            var productID = ids.GetType().GetProperty("ProductID");
            if (orderID == null || productID == null)
                return null;
            return await GetAll().SingleOrDefaultAsync(o => o.OrderID == (int)orderID.GetValue(ids) &&
                o.ProductID == (int)productID.GetValue(ids));
        }

        public IEnumerable<OrderDetail> GetForOrder(int id)
        {
            return GetAll().Where(o => o.OrderID == id);
        }

        public async Task Insert(OrderDetail order)
        {
            await EfDbSet.AddAsync(order);
        }

        public async Task Update(OrderDetail order)
        {
            var entry = Context.Entry(order);
            if (entry.State == EntityState.Detached)
            {
                var attachedOrder = await GetById(order.OrderID);
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

        public void Delete(OrderDetail order)
        {
            EfDbSet.Remove(order);
        }

        public void Save()
        {
            Context.SaveChanges();
        }
    }

    public interface IOrderDetailsRepository
    {
        Task Insert(OrderDetail order);
        Task Update(OrderDetail order);
        void Delete(OrderDetail order);
        void Save();
    }
}
