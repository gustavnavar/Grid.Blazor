using GridMvc.Pagination;
using GridMvc.Demo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GridMvc.Demo.Components
{
    public class AjaxGridViewComponent : ViewComponent
    {
        private readonly NorthwindDbContext _context;

        public AjaxGridViewComponent(NorthwindDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var repository = new OrdersRepository(_context);
            var model = new SGrid<Order>(repository.GetAll(), HttpContext.Request.Query, false, GridPager.DefaultAjaxPagerViewName);

            var factory = Task<IViewComponentResult>.Factory;
            return await factory.StartNew(() => View(model));
        }
    }
}
