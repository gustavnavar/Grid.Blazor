using GridCore.Pagination;
using GridMvc.Demo.Models;
using GridMvc.Server;
using GridShared;
using GridShared.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GridMvc.Demo.Components
{
    public class CustomersViewComponent : ViewComponent
    {
        private readonly NorthwindDbContext _context;

        public CustomersViewComponent(NorthwindDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string customersGridState = "")
        {
            IQueryCollection query = Request.Query;
            if (!string.IsNullOrWhiteSpace(customersGridState))
            {      
                try
                {
                    query = new QueryCollection(StringExtensions.GetQuery(customersGridState));
                }
                catch (Exception)
                {
                    // do nothing, gridState was not a valid state
                }
            }

            Action<IGridColumnCollection<Customer>> columns = c =>
            {
                /* Adding "CustomerID" column: */

                c.Add(o => o.CustomerID)
                    .Titled("ID");

                /* Adding "CompanyName" column: */
                c.Add(o => o.CompanyName)
                    .Titled("Company");

                /* Adding "ContactName" column: */
                c.Add(o => o.ContactName).Titled("ContactName").SetWidth(250);

                /* Adding "Address" column: */
                c.Add(o => o.Address)
                    .Titled("Address");

                /* Adding "City" column: */
                c.Add(o => o.City)
                    .Titled("City");

                /* Adding "PostalCode" column: */
                c.Add(o => o.PostalCode)
                    .Titled("Postal Code");

                /* Adding "Country" column: */
                c.Add(o => o.Country)
                    .Titled("Country");
            };

            var repository = new CustomersRepository(_context);
            var requestCulture = HttpContext.Features.Get<IRequestCultureFeature>();
            var locale = requestCulture.RequestCulture.UICulture.TwoLetterISOLanguageName;

            var server = new GridServer<Customer>(repository.GetAll(), query, false, "customersGrid",
                columns, 10, locale, GridPager.DefaultAjaxPagerViewName)
                .SetRowCssClasses(item => item.IsVip ? "success" : string.Empty)
                .Sortable()
                .Filterable()
                .WithMultipleFilters()
                .Searchable(true, false)
                .SetStriped(true)
                .ClearFiltersButton(true)
                .ChangePageSize(true)
                .WithGridItemsCount()
                .SetRemoveDiacritics<NorthwindDbContext>("RemoveDiacritics");
            var factory = Task<IViewComponentResult>.Factory;

            return await factory.StartNew(() => View(server.Grid));
        }
    }
}
