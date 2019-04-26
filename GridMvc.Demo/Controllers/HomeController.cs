using GridMvc.Demo.Models;
using GridMvc.Pagination;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Linq;

namespace GridMvc.Demo.Controllers
{
    public class HomeController : ApplicationController
    {
        private readonly NorthwindDbContext _context;

        public HomeController(NorthwindDbContext context, ICompositeViewEngine compositeViewEngine) : base(compositeViewEngine)
        {
            _context = context;
        }

        public ActionResult Index()
        {
            ViewBag.ActiveMenuTitle = "Demo";
            var repository = new OrdersRepository(_context);
            var orders = repository.GetAll();
            return View(orders);
        }

        public ActionResult About()
        {
            ViewBag.ActiveMenuTitle = "About";
            return View();
        }

        [HttpGet]
        public ActionResult GetOrder(int id)
        {
            var repository = new OrdersRepository(_context);
            Order order = repository.GetById(id);
            if (order == null)
                return Json(new { status = 0, message = "Not found" });

            return Json(new { status = 1, message = "Ok", content = RenderPartialViewToString("_OrderInfo", order) });
        }

        [HttpGet]
        public ActionResult GetCustomersNames()
        {
            var repository = new CustomersRepository(_context);
            var allItems = repository.GetAll().Select(c => c.CompanyName);
            return Json(new { items = allItems });
        }

        [HttpGet]
        public ActionResult AjaxPaging()
        {
            var repository = new OrdersRepository(_context);
            var model = new SGrid<Order>(repository.GetAll(), HttpContext.Request.Query, false, GridPager.DefaultAjaxPagerViewName);

            ViewBag.ActiveMenuTitle = "AjaxPaging";
            return View(model);
        }

        [HttpPost]
        public ActionResult GetOrdersGridRows()
        {
            var repository = new OrdersRepository(_context);
            var model = new SGrid<Order>(repository.GetAll(), HttpContext.Request.Query, false, GridPager.DefaultAjaxPagerViewName);

            return PartialView("_OrdersGrid", model);
        }

        [HttpGet]
        public ActionResult AjaxPagingAntiForgery()
        {
            ViewBag.ActiveMenuTitle = "AjaxPagingAntiForgery";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetOrdersGridRowsAntiForgery()
        {
            return ViewComponent("AjaxGrid");
        }

        //
        // POST /Account/SetLanguage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            try
            {
                Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                    new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
                );
            }
            catch (Exception)
            {
            }
            return LocalRedirect(returnUrl);
        }

    }
}