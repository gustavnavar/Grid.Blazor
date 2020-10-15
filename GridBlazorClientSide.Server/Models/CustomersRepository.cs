using GridBlazorClientSide.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GridBlazorClientSide.Server.Models
{
    public class CustomersRepository : SqlRepository<Customer>, ICustomerRepository
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

        public async Task Insert(Customer customer)
        {
            await EfDbSet.AddAsync(customer);
        }

        public async Task Update(Customer customer)
        {
            var entry = Context.Entry(customer);
            if (entry.State == EntityState.Detached)
            {
                var attachedOrder = await GetById(customer.CustomerID);
                if (attachedOrder != null)
                {
                    Context.Entry(attachedOrder).CurrentValues.SetValues(customer);
                }
                else
                {
                    entry.State = EntityState.Modified;
                }
            }
        }

        public void Delete(Customer customer)
        {
            EfDbSet.Remove(customer);
        }

        public void Save()
        {
            Context.SaveChanges();
        }
    }

    public interface ICustomerRepository
    {
        Task Insert(Customer customer);
        Task Update(Customer customer);
        void Delete(Customer customer);
        void Save();
    }
}