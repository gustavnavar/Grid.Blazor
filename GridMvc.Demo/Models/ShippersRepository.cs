using System.Linq;

namespace GridMvc.Demo.Models
{
    public class ShippersRepository : SqlRepository<Shipper>
    {
        public ShippersRepository(NorthwindDbContext context)
            : base(context)
        {
        }

        public override IQueryable<Shipper> GetAll()
        {
            return EfDbSet;
        }

        public override Shipper GetById(object id)
        {
            return GetAll().FirstOrDefault(c => c.ShipperID == (int)id);
        }
    }
}