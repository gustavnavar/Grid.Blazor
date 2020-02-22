using GridBlazorServerSide.Data;
using GridShared;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace GridBlazorServerSide.Services
{
    public class ProductService : IProductService
    {
        private readonly DbContextOptions<NorthwindDbContext> _options;

        public ProductService(DbContextOptions<NorthwindDbContext> options)
        {
            _options = options;
        }

        public IEnumerable<SelectItem> GetAllProducts()
        {
            using (var context = new NorthwindDbContext(_options))
            {
                ProductRepository repository = new ProductRepository(context);
                return repository.GetAll()
                    .Select(r => new SelectItem(r.ProductID.ToString(), r.ProductID.ToString() + " - " + r.ProductName))
                    .ToList();
            }
        }
    }

    public interface IProductService
    {
        IEnumerable<SelectItem> GetAllProducts();
    }
}
