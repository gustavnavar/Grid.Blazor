﻿using GridMvc.Demo.Components;
using GridMvc.Demo.Models;
using GridMvc.Demo.Resources;
using GridMvc.Pagination;
using GridMvc.Server;
using GridShared;
using GridShared.Filtering;
using GridShared.Sorting;
using GridShared.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GridMvc.Demo.Controllers
{
    public class HomeController : ApplicationController
    {
        private readonly OrdersRepository _orderRepository;
        private readonly OrderDetailsRepository _orderDetailsRepository;
        private readonly CustomersRepository _customersRepository;

        public HomeController(NorthwindDbContext context, ICompositeViewEngine compositeViewEngine) : base(compositeViewEngine)
        {
            _orderRepository = new OrdersRepository(context);
            _orderDetailsRepository = new OrderDetailsRepository(context);
            _customersRepository = new CustomersRepository(context);
        }

        public ActionResult Index(string gridState = "")
        {
            //string returnUrl = Request.Path;
            string returnUrl = "/Home/Index";

            IQueryCollection query = Request.Query;
            if (!string.IsNullOrWhiteSpace(gridState))
            {
                try
                {
                    query = new QueryCollection(StringExtensions.GetQuery(gridState));
                }
                catch (Exception)
                {
                    // do nothing, gridState was not a valid state
                }
            }

            ViewBag.ActiveMenuTitle = "Demo";

            var requestCulture = HttpContext.Features.Get<IRequestCultureFeature>();
            var locale = requestCulture.RequestCulture.UICulture.TwoLetterISOLanguageName;
            SharedResource.Culture = requestCulture.RequestCulture.UICulture;

            Action<IGridColumnCollection<Order>> columns = c =>
            {
                /* Adding not mapped column, that renders body, using inline Razor html helper */
                c.Add()
                    .Encoded(false)
                    .Sanitized(false)
                    .SetWidth(30)
                    .Css("hidden-xs") //hide on phones
                    .RenderComponentAs<ButtonCellViewComponent>(returnUrl);

                /* Adding "OrderID" column: */

                c.Add(o => o.OrderID)
                    .Titled(SharedResource.Number)
                    .SetWidth(100);

                /* Adding "OrderDate" column: */
                c.Add(o => o.OrderDate, "OrderCustomDate")
                    .Titled(SharedResource.OrderCustomDate)
                    .SortInitialDirection(GridSortDirection.Descending)
                    .ThenSortByDescending(o => o.OrderID)
                    .SetCellCssClassesContraint(o => o.OrderDate.HasValue && o.OrderDate.Value >= DateTime.Parse("1997-01-01") ? "red" : "")
                    .Format("{0:yyyy-MM-dd}")
                    .SetWidth(110)
                    .Max(true).Min(true);

                /* Adding "CompanyName" column: */
                c.Add(o => o.Customer.CompanyName)
                    .Titled(SharedResource.CompanyName)
                    .SetWidth(250)
                    .SetInitialFilter(GridFilterType.StartsWith, "a")
                    .SetFilterWidgetType("CustomCompanyNameFilterWidget")
                    .Max(true).Min(true);

                /* Adding "ContactName" column: */
                c.Add(o => o.Customer.ContactName).Titled(SharedResource.ContactName).SetWidth(250)
                    .Max(true).Min(true);

                /* Adding "Freight" column: */
                c.Add(o => o.Freight)
                    .Titled(SharedResource.Freight)
                    .SetWidth(100)
                    .Format("{0:F}")
                    .Sum(true).Average(true).Max(true).Min(true);

                /* Adding "Vip customer" column: */
                c.Add(o => o.Customer.IsVip)
                    .Titled(SharedResource.IsVip)
                    .SetWidth(70)
                    .Css("hidden-xs") //hide on phones
                    .RenderValueAs(o => o.Customer.IsVip ? "Yes" : "No");
            };

            var server = new GridServer<Order>(_orderRepository.GetAll(), query, false, "ordersGrid",
                columns, 10, locale)
                .SetRowCssClasses(item => item.Customer.IsVip ? "success" : string.Empty)
                .Sortable()
                .Filterable()
                .WithMultipleFilters()
                .Searchable(true, false)
                .Groupable(true)
                .Selectable(true)
                .ChangePageSize(true)
                .WithGridItemsCount();

            return View(server.Grid);
        }

        public ActionResult About()
        {
            ViewBag.ActiveMenuTitle = "About";
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> GetOrder(int id)
        {
            Order order = _orderRepository.GetById(id);
            if (order == null)
                return Json(new { status = 0, message = "Not found" });

            string content = await RenderAsync("_OrderInfo", order);
            return Json(new { status = 1, message = "Ok", content });
        }

        [HttpGet]
        public ActionResult GetCustomersNames()
        {
            var allItems = _customersRepository.GetAll().Select(c => c.CompanyName);
            return Json(new { items = allItems });
        }

        [HttpGet]
        public ActionResult Subgrid(string gridState = "")
        {
            //string returnUrl = Request.Path;
            string returnUrl = "/Home/Subgrid";
            ViewData["returnUrl"] = returnUrl;

            IQueryCollection query = Request.Query;
            if (!string.IsNullOrWhiteSpace(gridState))
            {
                try
                {
                    query = new QueryCollection(StringExtensions.GetQuery(gridState));
                }
                catch (Exception)
                {
                    // do nothing, gridState was not a valid state
                }
            }

            var model = new SGrid<Order>(_orderRepository.GetAll(), query, false, GridPager.DefaultAjaxPagerViewName);

            ViewBag.ActiveMenuTitle = "Subgrid";
            return View(model);
        }

        [HttpPost]
        public ActionResult GetOrdersGridRows()
        {
            //string returnUrl = Request.Path;
            string returnUrl = "/Home/Subgrid";
            ViewData["returnUrl"] = returnUrl;

            var model = new SGrid<Order>(_orderRepository.GetAll(), Request.Query, false, GridPager.DefaultAjaxPagerViewName);

            return PartialView("_OrdersGrid", model);
        }

        [HttpPost]
        public ActionResult GetSubgrid(int OrderId)
        {
            Action<IGridColumnCollection<OrderDetail>> columns = c =>
            {
                /* Adding "OrderID" column: */
                c.Add(o => o.OrderID)
                    .Titled("Order Number")
                    .SortInitialDirection(GridSortDirection.Descending)
                    .ThenSortByDescending(o => o.ProductID)
                    .SetWidth(100);

                /* Adding "ProductID" column: */
                c.Add(o => o.ProductID)
                    .Titled("Product Number")
                    .SetWidth(100);

                /* Adding "ProductName" column: */
                c.Add(o => o.Product.ProductName)
                    .Titled("Product Name")
                    .SetWidth(250);

                /* Adding "Quantity" column: */
                c.Add(o => o.Quantity)
                    .Titled("Quantity")
                    .SetCellCssClassesContraint(o => o.Quantity >= 50 ? "red" : "")
                    .SetWidth(100)
                    .Format("{0:F}");

                /* Adding "UnitPrice" column: */
                c.Add(o => o.UnitPrice)
                    .Titled("Unit Price")
                    .SetWidth(100)
                    .Format("{0:F}");
            };

            var requestCulture = HttpContext.Features.Get<IRequestCultureFeature>();
            var locale = requestCulture.RequestCulture.UICulture.TwoLetterISOLanguageName;
            var orderDetails = _orderDetailsRepository.GetForOrder(OrderId);

            var server = new GridServer<OrderDetail>(orderDetails, Request.Query,
                    false, "orderDetailsGrid" + OrderId.ToString(), columns, 10, locale)
                        .SetRowCssClasses(item => item.Quantity > 10 ? "success" : string.Empty)
                        .Sortable()
                        .Filterable()
                        .WithMultipleFilters()
                        .WithGridItemsCount();

            return PartialView("_SubGrid", server.Grid);
        }

        [HttpGet]
        public ActionResult MultipleGrids(string gridState = "", string altGridState = "")
        {
            ViewBag.ActiveMenuTitle = "MultipleGrids";

            ViewData["ordersGridState"] = gridState;
            ViewData["customersGridState"] = altGridState;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetOrdersGrid()
        {
            return ViewComponent("Orders");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetCustomersGrid()
        {
            return ViewComponent("Customers");
        }

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

        public ActionResult Edit(int? id, string returnUrl, string gridState, string altGridState = "", string error = "")
        {
            if (id == null || !id.HasValue)
            {
                return BadRequest();
            }
            Order order = _orderRepository.GetById(id.Value);
            if (order == null)
            {
                return NotFound();
            };

            ViewData["returnUrl"] = returnUrl;
            ViewData["gridState"] = gridState;
            ViewData["altGridState"] = altGridState;
            TempData["error"] = error;
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Order order, string returnUrl, string gridState, string altGridState = "")
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _orderRepository.Update(order);
                    _orderRepository.Save();

                    return LocalRedirect(WebUtility.UrlDecode(returnUrl) + "?gridState=" + gridState + "&altGridState=" + altGridState);
                }
                catch (Exception e)
                {
                    return RedirectToAction("Edit", "Home", new { id = order.OrderID, returnUrl, gridState, altGridState, error = e.Message.Replace('{', '(').Replace('}', ')') });
                }
            }

            return RedirectToAction("Edit", "Home", new { id = order.OrderID, returnUrl, gridState, altGridState, error = "ModelState is not valid" });
        }

    }
}
