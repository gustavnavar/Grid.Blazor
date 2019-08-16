using GridBlazor.Columns;
using GridBlazor.Filtering;
using GridBlazor.Pagination;
using GridBlazor.Sorting;
using GridShared.Columns;
using GridShared.Filtering;
using GridShared.Sorting;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace GridBlazor
{
    public class GridHeaderComponentBase<T> : ComponentBase
    {
        private const string ThClass = "grid-header";
        private const string ThStyle = "display:none;";

        private const string FilteredButtonCssClass = "filtered";
        private const string FilterButtonCss = "grid-filter-btn";

        private int _sequence = 0;
        protected bool _isVisible = false;
        protected List<ColumnFilterValue> _filterSettings;
        private bool _isColumnFiltered;
        protected string _url;
        protected StringValues _clearInitFilter;

        protected string _cssStyles;
        protected string _cssClass;
        protected string _cssFilterClass;
        protected string _cssSortingClass;

        protected RenderFragment FilterWidgetRender { get; set; }

        [CascadingParameter(Name = "GridComponent")]
        private GridComponent<T> GridComponent { get; set; }

        [Parameter]
        public IGridColumn Column { get; set; }
        [Parameter]
        public QueryStringFilterSettings FilterSettings { get; set; }
        [Parameter]
        public QueryStringSortSettings SortingSettings { get; set; }
        [Parameter]
        public IQueryDictionary<Type> Filters { get; set; }

        protected override void OnParametersSet()
        {
            //determine current column filter settings
            _filterSettings = new List<ColumnFilterValue>();
            if (FilterSettings.IsInitState(Column) && Column.InitialFilterSettings != ColumnFilterValue.Null)
            {
                _filterSettings.Add(Column.InitialFilterSettings);
            }
            else
            {
                _filterSettings.AddRange(FilterSettings.FilteredColumns.GetByColumn(Column));
            }

            _isColumnFiltered = _filterSettings.Any(r => r.FilterType != GridFilterType.Condition);

            //determine current url:
            var queryBuilder = new CustomQueryStringBuilder(FilterSettings.Query);

            var exceptQueryParameters = new List<string>
                {
                    QueryStringFilterSettings.DefaultTypeQueryParameter,
                    QueryStringFilterSettings.DefaultClearInitFilterQueryParameter
                };
            string pagerParameterName = GetPagerQueryParameterName(((ICGrid)(Column.ParentGrid)).Pager);
            if (!string.IsNullOrEmpty(pagerParameterName))
                exceptQueryParameters.Add(pagerParameterName);

            _url = queryBuilder.GetQueryStringExcept(exceptQueryParameters);

            _clearInitFilter = FilterSettings.Query.Get(QueryStringFilterSettings.DefaultClearInitFilterQueryParameter);

            if (((GridColumnBase<T>)Column).Hidden)
                _cssStyles = ((GridStyledColumn)Column).GetCssStylesString() + " " + ThStyle;
            else
                _cssStyles = ((GridStyledColumn)Column).GetCssStylesString();
            _cssClass = ((GridStyledColumn)Column).GetCssClassesString() + " " + ThClass;

            if (!string.IsNullOrWhiteSpace(Column.Width))
                _cssStyles = string.Concat(_cssStyles, " width:", Column.Width, ";").Trim();

            List<string> cssFilterClasses = new List<string>();
            cssFilterClasses.Add(FilterButtonCss);
            if (_isColumnFiltered)
                cssFilterClasses.Add(FilteredButtonCssClass);
            _cssFilterClass = string.Join(" ", cssFilterClasses);

            List<string> cssSortingClass = new List<string>();
            cssSortingClass.Add("grid-header-title");

            if (Column.IsSorted)
            {
                cssSortingClass.Add("sorted");
                cssSortingClass.Add(Column.Direction == GridSortDirection.Ascending ? "sorted-asc" : "sorted-desc");
            }
            _cssSortingClass = string.Join(" ", cssSortingClass);

            FilterWidgetRender = CreateFilterWidgetComponent();
        }

        private RenderFragment CreateFilterWidgetComponent() => builder =>
        {
            try
            {
                Type filterWidget = Filters[Column.FilterWidgetTypeName];
                builder.OpenComponent(++_sequence, filterWidget);
            }
            catch (Exception)
            {
                builder.OpenComponent<TextFilterComponent<T>>(++_sequence);
            }
            builder.AddAttribute(++_sequence, "GridHeaderComponent", this);
            builder.AddAttribute(++_sequence, "Visible", _isVisible);
            builder.AddAttribute(++_sequence, "ColumnName", Column.Name);
            builder.AddAttribute(++_sequence, "FilterSettings", _filterSettings);
            builder.CloseComponent();
        };

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

        protected async Task TitleButtonClicked()
        {
            //switch direction for link:
            GridSortDirection newDir = Column.Direction == GridSortDirection.Ascending
                                           ? GridSortDirection.Descending
                                           : GridSortDirection.Ascending;

            await GridComponent.GetSortUrl(SortingSettings.ColumnQueryParameterName, Column.Name, 
                SortingSettings.DirectionQueryParameterName, ((int)newDir).ToString(CultureInfo.InvariantCulture));
        }

        protected void FilterIconClicked()
        {
            //switch visibility for the filter dialog:
            _isVisible = !_isVisible;
            StateHasChanged();
        }

        public async Task AddFilter(FilterCollection filters)
        {
            _isVisible = !_isVisible;
            await GridComponent.AddFilter(Column, filters);
        }

        public async Task RemoveFilter()
        {
            _isVisible = !_isVisible;
            await GridComponent.RemoveFilter(Column);
        }
    }
}