using System.Linq;

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

        public override Customer GetById(object id)
        {
            return GetAll().FirstOrDefault(c => c.CustomerID == (string)id);
        }
    }
}