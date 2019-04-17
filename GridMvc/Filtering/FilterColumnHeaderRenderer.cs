using GridShared.Columns;
using GridShared.Filtering;
using GridShared.Utility;
using GridMvc.Pagination;
using GridMvc.Resources;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;

namespace GridMvc.Filtering
{
    /// <summary>
    ///     Renderer for sortable column
    /// </summary>
    internal class QueryStringFilterColumnHeaderRenderer : IGridColumnHeaderRenderer
    {
        private const string FilteredButtonCssClass = "filtered";
        private const string FilterButtonCss = "grid-filter-btn";

        private readonly QueryStringFilterSettings _settings;

        public QueryStringFilterColumnHeaderRenderer(QueryStringFilterSettings settings)
        {
            _settings = settings;
        }

        #region IGridColumnRenderer Members

        public IHtmlContent Render(IGridColumn column)
        {
            if (!column.FilterEnabled)
                return new HtmlString(string.Empty);


            //determine current column filter settings
            var filterSettings = new List<ColumnFilterValue>();
            if (_settings.IsInitState && column.InitialFilterSettings != ColumnFilterValue.Null)
            {
                filterSettings.Add(column.InitialFilterSettings);
            }
            else
            {
                filterSettings.AddRange(_settings.FilteredColumns.GetByColumn(column));
            }

            bool isColumnFiltered = filterSettings.Any();

            //determine current url:
            var builder = new CustomQueryStringBuilder(_settings.Query);

            var exceptQueryParameters = new List<string>
                {
                    QueryStringFilterSettings.DefaultTypeQueryParameter,
                    QueryStringFilterSettings.DefaultFilterInitQueryParameter
                };
            string pagerParameterName = GetPagerQueryParameterName(((ISGrid)column.ParentGrid).Pager);
            if (!string.IsNullOrEmpty(pagerParameterName))
                exceptQueryParameters.Add(pagerParameterName);

            string url = builder.GetQueryStringExcept(exceptQueryParameters);

            var gridFilterButton = new TagBuilder("span");
            gridFilterButton.AddCssClass(FilterButtonCss);
            if (isColumnFiltered)
                gridFilterButton.AddCssClass(FilteredButtonCssClass);
            gridFilterButton.Attributes.Add("title", Strings.FilterButtonTooltipText);

            var gridFilter = new TagBuilder("div");
            var dataKeyList = new Dictionary<string, string>
                {
                    {"data-type", column.FilterWidgetTypeName},
                    {"data-name", column.Name},
                    {"data-widgetdata", JsonHelper.JsonSerializer(column.FilterWidgetData)},
                    {"data-filterdata", JsonHelper.JsonSerializer(filterSettings)},
                    {"data-url", url}
                };

            using (var sw = new StringWriter())
            {
                gridFilterButton.WriteTo(sw, HtmlEncoder.Default);
                gridFilter.InnerHtml.SetHtmlContent(sw.ToString());
            }

            gridFilter.AddCssClass("grid-filter");
            foreach (var data in dataKeyList)
            {
                if (!string.IsNullOrWhiteSpace(data.Value))
                    gridFilter.Attributes.Add(data.Key, data.Value);
            }

            using (var sw = new StringWriter())
            {
                gridFilter.WriteTo(sw, HtmlEncoder.Default);
                return new HtmlString(sw.ToString());
            }
        }

        #endregion

        /// <summary>
        ///     Extract query string parameter name from default grid pager (if using)
        /// </summary>
        private string GetPagerQueryParameterName(IGridPager pager)
        {
            var defaultPager = pager as GridPager;
            if (defaultPager == null)
                return string.Empty;
            return defaultPager.ParameterName;
        }
    }
}