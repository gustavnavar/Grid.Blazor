using GridCore.Pagination;
using GridCore.Resources;
using GridMvc.Demo.Components;
using GridMvc.Demo.Models;
using GridMvc.Demo.Resources;
using GridMvc.Server;
using GridShared;
using GridShared.Filtering;
using GridShared.Sorting;
using GridShared.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
        private readonly ShippersRepository _shippersRepository;
        private readonly EmployeeRepository _employeeRepository;

        public HomeController(NorthwindDbContext context, ICompositeViewEngine compositeViewEngine) : base(compositeViewEngine)
        {
            _orderRepository = new OrdersRepository(context);
            _orderDetailsRepository = new OrderDetailsRepository(context);
            _customersRepository = new CustomersRepository(context);
            _shippersRepository = new ShippersRepository(context);
            _employeeRepository = new EmployeeRepository(context);
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

            var shipperList = _shippersRepository.GetAll()
                .Select(s => new SelectItem(s.ShipperID.ToString(), s.CompanyName))
                .ToList();

            Action<IGridColumnCollection<Order>> columns = c =>
            {
                /* Adding not mapped column, that renders body, using inline Razor html helper */
                c.Add()
                    .Encoded(false)
                    .Sanitized(false)
                    .SetWidth(60)
                    .Css("hidden-xs") //hide on phones
                    .RenderComponentAs<ButtonCellViewComponent>(returnUrl);

                /* Adding "OrderID" column: */

                c.Add(o => o.OrderID)
                    .Titled(SharedResource.Number)
                    .SetWidth(100)
                    .Sum(true);

                /* Adding "OrderDate" column: */
                c.Add(o => o.OrderDate, "OrderCustomDate")
                    .Titled(SharedResource.OrderCustomDate)
                    .SortInitialDirection(GridSortDirection.Descending)
                    .ThenSortByDescending(o => o.OrderID)
                    .SetCellCssClassesContraint(o => o.OrderDate.HasValue && o.OrderDate.Value >= DateTime.Parse("1997-01-01") ? "red" : "")
                    .Format("{0:yyyy-MM-dd}")
                    .SetWidth(110)
                    .Max(true).Min(true);

                c.Add(o => o.ShipVia)
                    .Titled("Via")
                    .SetWidth(250)
                    .RenderValueAs(o => o.Shipper?.CompanyName)
                    .SetListFilter(shipperList);

                /* Adding "CompanyName" column: */
                c.Add(o => o.Customer.CompanyName)
                    .Titled(SharedResource.CompanyName)
                    .ThenSortByDescending(o => o.ShipVia)
                    .ThenSortByDescending(o => o.Freight)
                    .SetWidth(250)
                    .SetInitialFilter(GridFilterType.StartsWith, "a")
                    .SetFilterWidgetType("CustomCompanyNameFilterWidget")
                    .Max(true).Min(true);

                /* Adding "ContactName" column: */
                c.Add(o => o.Customer.ContactName).Titled(SharedResource.ContactName).SetWidth(250)
                    .Max(true).Min(true);

                /* Adding "Customer.Country" hidden column: */
                c.Add(o => o.Customer.Country, true);

                /* Adding "Freight" column: */
                c.Add(o => o.Freight)
                    .Titled(SharedResource.Freight)
                    .SetWidth(100)
                    .Format("{0:F}")
                    .Sum(true).Average(true).Max(true).Min(true)
                    .Calculate("Average 2", x => x.Grid.ItemsCount == 0 ? 0 : x.Get("Freight").SumValue.Number / x.Grid.ItemsCount)
                    .Calculate("Average 3", x => x.Get("OrderID").SumValue.Number == 0 ? 0 : x.Get("Freight").SumValue.Number / x.Get("OrderID").SumValue.Number);

                /* Adding "Vip customer" column: */
                c.Add(o => o.Customer.IsVip)
                    .Titled(SharedResource.IsVip)
                    .SetWidth(80)
                    .Css("hidden-xs") //hide on phones
                    .RenderValueAs(o => o.Customer.IsVip ? Strings.BoolTrueLabel : Strings.BoolFalseLabel);
            };

            var server = new GridServer<Order>(_orderRepository.GetAll(), query, false, "ordersGrid",
                columns, 10, locale)
                .SetRowCssClasses(item => item.Customer.IsVip ? "success" : string.Empty)
                .Sortable()
                .Filterable()
                .WithMultipleFilters()
                .Searchable(true, false)
                .Groupable(true)
                .ClearFiltersButton(true)
                .Selectable(true)
                .SetStriped(true)
                .ChangePageSize(true)
                .WithGridItemsCount()
                .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics")
                .SetToListAsyncFunc(async x => await x.ToListAsync())
                .SyncButton(true);

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
            Order order = await _orderRepository.GetById(id);
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

            var shipperList = _shippersRepository.GetAll()
                .Select(s => new SelectItem(s.ShipperID.ToString(), s.CompanyName))
                .ToList();
            ViewData["shipperList"] = shipperList;

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

            var model = new SGrid<Order>(_orderRepository.GetAll().Include(r => r.OrderDetails), query, false, GridPager.DefaultAjaxPagerViewName);

            ViewBag.ActiveMenuTitle = "Subgrid";
            return View(model);
        }

        [HttpPost]
        public ActionResult GetOrdersGridRows()
        {
            //string returnUrl = Request.Path;
            string returnUrl = "/Home/Subgrid";
            ViewData["returnUrl"] = returnUrl;

            var shipperList = _shippersRepository.GetAll()
                .Select(s => new SelectItem(s.ShipperID.ToString(), s.CompanyName))
                .ToList();
            ViewData["shipperList"] = shipperList;

            var model = new SGrid<Order>(_orderRepository.GetAll().Include(r => r.OrderDetails), Request.Query, false, GridPager.DefaultAjaxPagerViewName);

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
                        .SetStriped(true)
                        .WithMultipleFilters()
                        .WithGridItemsCount()
                        .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            return PartialView("_SubGrid", server.Grid);
        }

        [HttpGet]
        public ActionResult RTL(string gridState = "")
        {
            //string returnUrl = Request.Path;
            string returnUrl = "/Home/RTL";
            ViewData["returnUrl"] = returnUrl;

            var shipperList = _shippersRepository.GetAll()
                .Select(s => new SelectItem(s.ShipperID.ToString(), s.CompanyName))
                .ToList();
            ViewData["shipperList"] = shipperList;

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

            ViewBag.ActiveMenuTitle = "RTL";
            return View(model);
        }

        [HttpPost]
        public ActionResult GetOrdersRTLGridRows()
        {
            //string returnUrl = Request.Path;
            string returnUrl = "/Home/RTL";
            ViewData["returnUrl"] = returnUrl;

            var shipperList = _shippersRepository.GetAll()
                .Select(s => new SelectItem(s.ShipperID.ToString(), s.CompanyName))
                .ToList();
            ViewData["shipperList"] = shipperList;

            var model = new SGrid<Order>(_orderRepository.GetAll(), Request.Query, false, GridPager.DefaultAjaxPagerViewName);

            return PartialView("_OrdersRTLGrid", model);
        }

        [HttpPost]
        public ActionResult GetSubgridRTL(int OrderId)
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
                        .SetStriped(true)
                        .WithMultipleFilters()
                        .WithGridItemsCount()
                        .SetDirection(GridDirection.RTL)
                        .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

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

        public async Task<ActionResult> Edit(int? id, string returnUrl, string gridState, string altGridState = "", string error = "")
        {
            if (id == null || !id.HasValue)
            {
                return BadRequest();
            }
            Order order = await _orderRepository.GetById(id.Value);
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
        public async Task<ActionResult> Edit(Order order, string returnUrl, string gridState, string altGridState = "")
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _orderRepository.Update(order);
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

        public ActionResult BlazorComponentView()
        {
            return View();
        }

        public ActionResult Comparer(string gridState = "")
        {
            //string returnUrl = Request.Path;
            string returnUrl = "/Home/Comparer";

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

            var shipperList = _shippersRepository.GetAll()
                .Select(s => new SelectItem(s.ShipperID.ToString(), s.CompanyName))
                .ToList();

            var comparer = new SampleComparer(StringComparer.OrdinalIgnoreCase);

            Action<IGridColumnCollection<Order>> columns = c =>
            {
                /* Adding not mapped column, that renders body, using inline Razor html helper */
                c.Add()
                    .Encoded(false)
                    .Sanitized(false)
                    .SetWidth(60)
                    .Css("hidden-xs") //hide on phones
                    .RenderComponentAs<ButtonCellViewComponent>(returnUrl);

                /* Adding "OrderID" column: */

                c.Add(o => o.OrderID)
                    .Titled(SharedResource.Number)
                    .SetWidth(100);

                /* Adding "OrderDate" column: */
                c.Add(o => o.OrderDate, "OrderCustomDate")
                    .Titled(SharedResource.OrderCustomDate)
                    .SetCellCssClassesContraint(o => o.OrderDate.HasValue && o.OrderDate.Value >= DateTime.Parse("1997-01-01") ? "red" : "")
                    .Format("{0:yyyy-MM-dd}")
                    .SetWidth(110)
                    .Max(true).Min(true);

                c.Add(o => o.ShipVia)
                    .Titled("Via")
                    .SetWidth(250)
                    .RenderValueAs(o => o.Shipper?.CompanyName)
                    .SetListFilter(shipperList);

                /* Adding "CompanyName" column: */
                c.Add(o => o.Customer.CompanyName, comparer)
                    .Titled(SharedResource.CompanyName)
                    .SetWidth(250)
                    .SetFilterWidgetType("CustomCompanyNameFilterWidget")
                    .Max(true).Min(true);

                /* Adding "ContactName" column: */
                c.Add(o => o.Customer.ContactName).Titled(SharedResource.ContactName).SetWidth(250)
                    .Max(true).Min(true);

                /* Adding "Customer.Country" hidden column: */
                c.Add(o => o.Customer.Country, true);

                /* Adding "Freight" column: */
                c.Add(o => o.Freight)
                    .Titled(SharedResource.Freight)
                    .SetWidth(100)
                    .Format("{0:F}")
                    .Sum(true).Average(true).Max(true).Min(true);

                /* Adding "Vip customer" column: */
                c.Add(o => o.Customer.IsVip)
                    .Titled(SharedResource.IsVip)
                    .SetWidth(80)
                    .Css("hidden-xs") //hide on phones
                    .RenderValueAs(o => o.Customer.IsVip ? Strings.BoolTrueLabel : Strings.BoolFalseLabel);
            };

            var server = new GridServer<Order>(_orderRepository.GetAll().ToList(), query, false, "ordersGrid",
                columns, 10, locale)
                .SetRowCssClasses(item => item.Customer.IsVip ? "success" : string.Empty)
                .Sortable()
                .Filterable()
                .WithMultipleFilters()
                .Searchable(true, false)
                .Groupable(true)
                .ClearFiltersButton(true)
                .Selectable(true)
                .SetStriped(true)
                .ChangePageSize(true)
                .WithGridItemsCount()
                .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");

            return View(server.Grid);
        }

        public ActionResult Images()
        {
            IQueryCollection query = Request.Query;
            ViewBag.ActiveMenuTitle = "Image Demo";

            var requestCulture = HttpContext.Features.Get<IRequestCultureFeature>();
            var locale = requestCulture.RequestCulture.UICulture.TwoLetterISOLanguageName;
            SharedResource.Culture = requestCulture.RequestCulture.UICulture;

            Action<IGridColumnCollection<Employee>> columns = c =>
            {
                c.Add(o => o.EmployeeID).Titled(SharedResource.Number).SetWidth("10%");
                c.Add(o => o.Title).SetWidth("5%");
                c.Add(o => o.FirstName).SetWidth("40%");
                c.Add(o => o.LastName).SetWidth("40%");
                c.Add().Encoded(false).Sanitized(false).SetWidth("5%")
                    .RenderValueAs(o => $"<img width='50' height='50' src='data:image/bmp;base64,{o.Base64String}' />");
            };

            var server = new GridServer<Employee>(_employeeRepository.GetAll(), query, false, "employeesGrid",
                columns, 10, locale)
                .Sortable()
                .Filterable()
                .WithMultipleFilters()
                .Searchable(true, false)
                .Groupable(true)
                .ClearFiltersButton(true)
                .Selectable(true)
                .SetStriped(true)
                .ChangePageSize(true)
                .WithGridItemsCount()
                .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics")
                .SetToListAsyncFunc(async x => await x.ToListAsync());

            return View(server.Grid);
        }

        [HttpGet]
        public ActionResult Trucks()
        {
            IQueryCollection query = Request.Query;

            ViewBag.ActiveMenuTitle = "Trucks";

            var requestCulture = HttpContext.Features.Get<IRequestCultureFeature>();
            var locale = requestCulture.RequestCulture.UICulture.TwoLetterISOLanguageName;
            SharedResource.Culture = requestCulture.RequestCulture.UICulture;

            IEnumerable<SelectItem> Types = new PersonType[] { PersonType.Driver, PersonType.Owner, PersonType.DriverAndOwner }
               .Select(r => new SelectItem(r.ToString(), r.ToText()));

            Action<IGridColumnCollection<Truck>> columns = c =>
            {
                c.Add(t => t.Description).Titled("Description");
                c.Add(t => t.Person.FullName).Titled("Person");
                c.Add(t => t.Date).Titled("Date");
                c.Add(t => t.Time).Titled("Time");
                c.Add(t => t.Type).Titled("Type").RenderValueAs(r => r.Type?.ToText()).SetListFilter(Types, true, true);
            };

            var server = new GridServer<Truck>(GetAllTrucks(), query, false, "trucksGrid",
                columns, 10, locale)
                .Sortable()
                .Filterable()
                .WithMultipleFilters()
                .Searchable(true, false)
                .Groupable(true)
                .ClearFiltersButton(true)
                .Selectable(true)
                .SetStriped(true)
                .ChangePageSize(true)
                .WithGridItemsCount()
                .SyncButton(true);

            return View(server.Grid);
        }

        private IEnumerable<Truck> GetAllTrucks()
        {
            var trucks = new List<Truck>();
            trucks.Add(new Truck
            {
                Id = 1,
                Description = "Truck 1",
                Date = new DateOnly(2021, 1, 1),
                Time = new TimeOnly(12, 30),
                Person = new Person
                {
                    Id = 1,
                    FirstName = "Person",
                    LastName = "1"
                },
                Type = PersonType.DriverAndOwner
            });
            trucks.Add(new Truck
            {
                Id = 2,
                Description = "Truck 2",
                Date = new DateOnly(2021, 1, 4),
                Time = new TimeOnly(14, 30),
                Person = new Person
                {
                    Id = 2,
                    LastName = "2"
                },
                Type = PersonType.Driver
            });
            trucks.Add(new Truck
            {
                Id = 3,
                Description = "Truck 3",
                Date = new DateOnly(2021, 1, 8),
                Time = new TimeOnly(16, 30),
                Person = new Person
                {
                    Id = 1,
                    FirstName = "Person"
                },
                Type = PersonType.Owner
            });
            trucks.Add(new Truck
            {
                Id = 4,
                Description = "Truck 4",
                Date = new DateOnly(2021, 1, 12),
                Time = new TimeOnly(18, 30),
                Person = new Person
                {
                    Id = 4,
                    FirstName = "Person",
                    LastName = "4"
                },
                Type = PersonType.Driver
            });
            trucks.Add(new Truck
            {
                Id = 5,
                Description = "Truck 5",
                Date = new DateOnly(2021, 1, 16),
                Time = new TimeOnly(20, 30),
                Person = new Person
                {
                    Id = 5,
                    FirstName = "Person",
                    LastName = "5"
                },
                Type = PersonType.Driver
            });
            trucks.Add(new Truck
            {
                Id = 6,
                Description = "Truck 6",
                Date = new DateOnly(2021, 1, 20),
                Time = new TimeOnly(22, 30),
                Person = new Person
                {
                    Id = 6,
                    FirstName = "Person",
                    LastName = "6"
                },
                Type = PersonType.Owner
            });
            return trucks;
        }
    }
}