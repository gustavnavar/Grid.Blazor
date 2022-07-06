using GridCore.Pagination;
using GridCore.Resources;
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
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GridMvc.Demo.Components
{
    public class OrdersViewComponent : ViewComponent
    {
        private readonly NorthwindDbContext _context;

        public OrdersViewComponent(NorthwindDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string ordersGridState = "")
        {
            IQueryCollection query = Request.Query;
            if (!string.IsNullOrWhiteSpace(ordersGridState))
            {      
                try
                {
                    query = new QueryCollection(StringExtensions.GetQuery(ordersGridState));
                }
                catch (Exception)
                {
                    // do nothing, gridState was not a valid state
                }
            }

            var requestCulture = HttpContext.Features.Get<IRequestCultureFeature>();
            var locale = requestCulture.RequestCulture.UICulture.TwoLetterISOLanguageName;
            SharedResource.Culture = requestCulture.RequestCulture.UICulture;

            var shippersRepository = new ShippersRepository(_context);
            var shipperList = shippersRepository.GetAll()
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
                    .RenderValueAs(o => $"<b><a class='modal_link' href='javascript:void(0);' onclick='editOrder({o.OrderID})'>Edit</a></b>");

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
                    .Sum(true).Average(true).Max(true).Min(true)
                    .Calculate("Average 2", x => x.Get("Freight").SumValue.Number / x.Grid.ItemsCount)
                    .Calculate("Average 3", x => x.Get("Freight").SumValue.Number / x.Get("OrderID").SumValue.Number);


                /* Adding "Vip customer" column: */
                c.Add(o => o.Customer.IsVip)
                    .Titled(SharedResource.IsVip)
                    .SetWidth(80)
                    .Css("hidden-xs") //hide on phones
                    .RenderValueAs(o => o.Customer.IsVip ? Strings.BoolTrueLabel : Strings.BoolFalseLabel);
            };

            var repository = new OrdersRepository(_context);

            var server = new GridServer<Order>(repository.GetAll(), query, false, "ordersGrid",
                columns, 10, locale, GridPager.DefaultAjaxPagerViewName)
                .SetRowCssClasses(item => item.Customer.IsVip ? "success" : string.Empty)
                .Sortable()
                .Filterable()
                .WithMultipleFilters()
                .Searchable(true, false)
                .Groupable(true)
                .Selectable(true)
                .SetStriped(true)
                .ChangePageSize(true)
                .WithGridItemsCount()
                .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");
            var factory = Task<IViewComponentResult>.Factory;

            return await factory.StartNew(() => View(server.Grid));
        }
    }
}
