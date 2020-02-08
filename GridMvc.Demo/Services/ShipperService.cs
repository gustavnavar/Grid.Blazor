using GridMvc.Demo.Models;
using GridShared;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace GridMvc.Demo.Services
{
    public class ShipperService : IShipperService
    {
        private readonly DbContextOptions<NorthwindDbContext> _options;

        public ShipperService(DbContextOptions<NorthwindDbContext> options)
        {
            _options = options;
        }

        public IEnumerable<SelectItem> GetAllShippers()
        {
            using (var context = new NorthwindDbContext(_options))
            {
                ShippersRepository repository = new ShippersRepository(context);
                return repository.GetAll()
                    .Select(r => new SelectItem(r.ShipperID.ToString(), r.ShipperID.ToString() + " - "
                        + r.CompanyName))
                    .ToList();
            }
        }
    }

    public interface IShipperService
    {
        IEnumerable<SelectItem> GetAllShippers();
    }
}
