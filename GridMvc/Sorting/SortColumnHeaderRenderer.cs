using GridShared.Columns;
using GridShared.Sorting;
using GridShared.Utility;
using GridMvc.Pagination;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using System.IO;
using System.Text.Encodings.Web;

namespace GridMvc.Sorting
{
    /// <summary>
    ///     Renderer for sortable column.
    ///     Object renders column name as link
    /// </summary>
    internal class QueryStringSortColumnHeaderRenderer : IGridColumnHeaderRenderer
    {
        private readonly QueryStringSortSettings _settings;

        public QueryStringSortColumnHeaderRenderer(QueryStringSortSettings settings)
        {
            _settings = settings;
        }

        public IHtmlContent Render(IGridColumn column)
        {
            return new HtmlString(GetSortHeaderContent(column));
        }

        protected string GetSortHeaderContent(IGridColumn column)
        {
            
            var sortTitle = new TagBuilder("div");
            sortTitle.AddCssClass("grid-header-title");

            if (column.SortEnabled)
            {
                var columnHeaderLink = new TagBuilder("a");
                columnHeaderLink.InnerHtml.SetHtmlContent(column.Title);
                string url = GetSortUrl(column.Name, column.Direction);
                columnHeaderLink.Attributes.Add("href", url);
                using (var sw = new StringWriter())
                {
                    columnHeaderLink.WriteTo(sw, HtmlEncoder.Default);
                    sortTitle.InnerHtml.AppendHtml(sw.ToString());
                }
            }
            else
            {
                var columnTitle = new TagBuilder("span");
                columnTitle.InnerHtml.SetHtmlContent(column.Title);
                using (var sw = new StringWriter())
                {
                    columnTitle.WriteTo(sw, HtmlEncoder.Default);
                    sortTitle.InnerHtml.SetHtmlContent(sw.ToString());
                }
            }

            if (column.IsSorted)
            {
                sortTitle.AddCssClass("sorted");
                sortTitle.AddCssClass(column.Direction == GridSortDirection.Ascending ? "sorted-asc" : "sorted-desc");

                var sortArrow = new TagBuilder("span");
                sortArrow.AddCssClass("grid-sort-arrow");

                using (var sw = new StringWriter())
                {
                    sortArrow.WriteTo(sw, HtmlEncoder.Default);
                    sortTitle.InnerHtml.AppendHtml(sw.ToString());
                }
            }

            using (var sw = new StringWriter())
            {
                sortTitle.WriteTo(sw, HtmlEncoder.Default);
                return sw.ToString();
            }
        }

        private string GetSortUrl(string columnName, GridSortDirection? direction)
        {
            //switch direction for link:
            GridSortDirection newDir = direction == GridSortDirection.Ascending
                                           ? GridSortDirection.Descending
                                           : GridSortDirection.Ascending;
            //determine current url:
            var builder = new CustomQueryStringBuilder(_settings.Query);
            string url =
                builder.GetQueryStringExcept(new[]
                    {
                        GridPager.DefaultPageQueryParameter,
                        _settings.ColumnQueryParameterName,
                        _settings.DirectionQueryParameterName
                    });
            if (string.IsNullOrEmpty(url))
                url = "?";
            else
                url += "&";
            return string.Format("{0}{1}={2}&{3}={4}", url, _settings.ColumnQueryParameterName, columnName,
                                 _settings.DirectionQueryParameterName,
                                 ((int) newDir).ToString(CultureInfo.InvariantCulture));
        }
    }
}