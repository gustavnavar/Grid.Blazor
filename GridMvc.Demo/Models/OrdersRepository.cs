using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GridMvc.Demo.Models
{
    public class OrdersRepository : SqlRepository<Order>, IOrdersRepository
    {
        public OrdersRepository(NorthwindDbContext context)
            : base(context)
        {
        }

        public override IQueryable<Order> GetAll()
        {
            return EfDbSet.Include("Customer").Include("Shipper").Include("Employee");
        }

        public override async Task<Order> GetById(object id)
        {
            return await GetAll().SingleOrDefaultAsync(o => o.OrderID == (int)id);
        }

        public async Task Insert(Order order)
        {
            await EfDbSet.AddAsync(order);
        }

        public async Task Update(Order order)
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

        public void Delete(Order order)
        {
            EfDbSet.Remove(order);
        }

        public void Save()
        {
            Context.SaveChanges();
        }
    }

    public interface IOrdersRepository
    {
        Task Insert(Order order);
        Task Update(Order order);
        void Delete(Order order);
        void Save();
    }
}