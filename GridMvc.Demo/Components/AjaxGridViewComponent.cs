using GridMvc.Demo.Models;
using GridMvc.Server;
using GridShared;
using GridShared.Filtering;
using GridShared.Sorting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System;
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
            Action<IGridColumnCollection<Order>> columns = c =>
            {
                /* Adding not mapped column, that renders body, using inline Razor html helper */
                c.Add()
                    .Encoded(false)
                    .Sanitized(false)
                    .SetWidth(30)
                    .Css("hidden-xs") //hide on phones
                    .RenderValueAs(o => $"<b><a class='modal_link' href='/Home/Edit/{o.OrderID}'>Edit</a></b>");

                /* Adding "OrderID" column: */

                c.Add(o => o.OrderID)
                    .Titled("Number")
                    .SetWidth(100);

                /* Adding "OrderDate" column: */
                c.Add(o => o.OrderDate, "OrderCustomDate")
                    .Titled("Date")
                    .SortInitialDirection(GridSortDirection.Descending)
                    .SetCellCssClassesContraint(o => o.OrderDate.HasValue && o.OrderDate.Value >= DateTime.Parse("1997-01-01") ? "red" : "")
                    .Format("{0:yyyy-MM-dd}")
                    .SetWidth(110)
                    .Max(true).Min(true);

                /* Adding "CompanyName" column: */
                c.Add(o => o.Customer.CompanyName)
                    .Titled("Company")
                    .SetWidth(250)
                    .ThenSortByDescending(o => o.OrderID)
                    .SetInitialFilter(GridFilterType.StartsWith, "a")
                    .SetFilterWidgetType("CustomCompanyNameFilterWidget")
                    .Max(true).Min(true);

                /* Adding "ContactName" column: */
                c.Add(o => o.Customer.ContactName).Titled("ContactName").SetWidth(250)
                    .Max(true).Min(true);

                /* Adding "Freight" column: */
                c.Add(o => o.Freight)
                    .Titled("Freight")
                    .SetWidth(100)
                    .Format("{0:F}")
                    .Sum(true).Average(true).Max(true).Min(true);

                /* Adding "Vip customer" column: */
                c.Add(o => o.Customer.IsVip)
                    .Titled("Is Vip")
                    .SetWidth(70)
                    .Css("hidden-xs") //hide on phones
                    .RenderValueAs(o => o.Customer.IsVip ? "Yes" : "No");
            };

            var repository = new OrdersRepository(_context);
            var requestCulture = HttpContext.Features.Get<IRequestCultureFeature>();
            var locale = requestCulture.RequestCulture.UICulture.TwoLetterISOLanguageName;

            var server = new GridServer<Order>(repository.GetAll(), Request.Query, false, "ordersGrid",
                columns, 10, locale)
                .SetRowCssClasses(item => item.Customer.IsVip ? "success" : string.Empty)
                .Sortable()
                .Filterable()
                .WithMultipleFilters()
                .Searchable(true, false)
                .WithGridItemsCount();
            var factory = Task<IViewComponentResult>.Factory;

            return await factory.StartNew(() => View(server.Grid));
        }
    }
}
