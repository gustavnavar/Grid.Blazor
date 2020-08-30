using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GridMvc.Demo.Models
{
    public class EmployeeRepository : SqlRepository<Employee>, IEmployeeRepository
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

        public async Task Insert(Employee employee)
        {
            await EfDbSet.AddAsync(employee);
        }

        public async Task Update(Employee employee)
        {
            var entry = Context.Entry(employee);
            if (entry.State == EntityState.Detached)
            {
                var attachedOrder = await GetById(employee.EmployeeID);
                if (attachedOrder != null)
                {
                    Context.Entry(attachedOrder).CurrentValues.SetValues(employee);
                }
                else
                {
                    entry.State = EntityState.Modified;
                }
            }
        }

        public void Delete(Employee employee)
        {
            EfDbSet.Remove(employee);
        }

        public void Save()
        {
            Context.SaveChanges();
        }
    }

    public interface IEmployeeRepository
    {
        Task Insert(Employee employee);
        Task Update(Employee employee);
        void Delete(Employee employee);
        void Save();
    }
}