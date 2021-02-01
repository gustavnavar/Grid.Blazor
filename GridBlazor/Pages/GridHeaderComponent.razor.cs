using GridBlazor.Filtering;
using GridBlazor.Pagination;
using GridBlazor.Sorting;
using GridShared;
using GridShared.Columns;
using GridShared.Events;
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

namespace GridBlazor.Pages
{
    public partial class GridHeaderComponent<T>
    {
        private const string ThClass = "grid-header";
        private const string ThStyle = "display:none;";

        private const string FilteredButtonCssClass = "filtered";
        private const string FilterButtonCss = "grid-filter-btn";

        private int _sequence = 0;
        protected bool _isFilterVisible = false;
        protected bool _isTooltipVisible = false;
        protected List<ColumnFilterValue> _filterSettings;
        private bool _isColumnFiltered;
        protected string _url;
        protected StringValues _clearInitFilter;
        private bool? _allChecked = null;
        private bool _showAllChecked = false;

        protected string _cssStyles;
        protected string _cssClass;
        protected string _cssFilterClass;
        protected string _cssSortingClass;

        protected RenderFragment FilterWidgetRender { get; set; }

        [CascadingParameter(Name = "GridComponent")]
        internal GridComponent<T> GridComponent { get; set; }

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

            if (Column.Hidden)
                _cssStyles = ((GridStyledColumn)Column).GetCssStylesString() + " " + ThStyle;
            else
                _cssStyles = ((GridStyledColumn)Column).GetCssStylesString();
            _cssClass = ((GridStyledColumn)Column).GetCssClassesString() + " " + ThClass;

            if (GridComponent.Grid.Direction == GridDirection.RTL)
                _cssStyles = string.Concat(_cssStyles, " text-align:right;direction:rtl;").Trim();

            // tables with fixed layout don't need to set up column width on the header
            if (GridComponent.Grid.TableLayout == TableLayout.Auto)
            {
                if (!string.IsNullOrWhiteSpace(Column.Width))
                    _cssStyles = string.Concat(_cssStyles, " width:", Column.Width, ";").Trim();
            }

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

            if (Column.FilterEnabled)
                FilterWidgetRender = CreateFilterWidgetComponent();

            if (!string.IsNullOrWhiteSpace(Column.Name))
            {
                if (GridComponent.HeaderComponents.ContainsKey(Column.Name))
                {
                    var headerComponent = GridComponent.HeaderComponents.Get(Column.Name);
                    _allChecked = headerComponent._allChecked;
                    _showAllChecked = headerComponent._showAllChecked;
                }
                GridComponent.HeaderComponents.AddParameter(Column.Name, this);
            }
        }

        private RenderFragment CreateFilterWidgetComponent() => builder =>
        {
            builder.OpenComponent<CascadingValue<GridHeaderComponent<T>>>(++_sequence);
            builder.AddAttribute(++_sequence, "Value", this);
            builder.AddAttribute(++_sequence, "Name", "GridHeaderComponent");
            builder.AddAttribute(++_sequence, "ChildContent", CreateFilterChildContent());
            builder.CloseComponent();
        };

        private RenderFragment CreateFilterChildContent() => builder =>
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
            builder.AddAttribute(++_sequence, "Visible", _isFilterVisible);
            builder.AddAttribute(++_sequence, "ColumnName", Column.Name);
            builder.AddAttribute(++_sequence, "FilterSettings", _filterSettings);
            builder.CloseComponent();
        };

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                GridComponent.FilterButtonClicked += HideFilter;
                GridComponent.RowCheckboxChanged += RowCheckboxChanged;
            }
        }

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
            if (Column.IsSorted)
            {
                if (Column.Direction == GridSortDirection.Ascending)
                {
                    await GridComponent.GetSortUrl(SortingSettings.ColumnQueryParameterName, Column.Name, SortingSettings.DirectionQueryParameterName,
                        ((int)GridSortDirection.Descending).ToString(CultureInfo.InvariantCulture));
                }
                else
                {
                    if (Column.InitialDirection.HasValue)
                    {
                        await GridComponent.GetSortUrl(SortingSettings.ColumnQueryParameterName, Column.Name, SortingSettings.DirectionQueryParameterName,
                        ((int)GridSortDirection.Ascending).ToString(CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        await GridComponent.GetSortUrl(SortingSettings.ColumnQueryParameterName, SortingSettings.DirectionQueryParameterName);
                    }
                }
            }
            else
            {
                await GridComponent.GetSortUrl(SortingSettings.ColumnQueryParameterName, Column.Name, SortingSettings.DirectionQueryParameterName, 
                    ((int)GridSortDirection.Ascending).ToString(CultureInfo.InvariantCulture));
            }
        }

        public async Task FilterIconClicked()
        {
            var isVisible = _isFilterVisible;
            GridComponent.FilterIconClicked();

            //switch visibility for the filter dialog:
            _isFilterVisible = !isVisible;
                
            StateHasChanged();
            await GridComponent.SetGridFocus();
        }

        public async Task DisplayTooltip()
        {
            if (!string.IsNullOrWhiteSpace(Column.TooltipValue))
            {
                _isTooltipVisible = true;
                StateHasChanged();
                await GridComponent.SetGridFocus();
            }
        }

        public async Task HideTooltip()
        {
            if (!string.IsNullOrWhiteSpace(Column.TooltipValue))
            {
                _isTooltipVisible = false;
                StateHasChanged();
                await GridComponent.SetGridFocus();
            }
        }

        public async Task AddFilter(FilterCollection filters)
        {
            _isFilterVisible = !_isFilterVisible;
            StateHasChanged();
            await GridComponent.AddFilter(Column, filters);
        }

        public async Task RemoveFilter()
        {
            _isFilterVisible = !_isFilterVisible;
            StateHasChanged();
            await GridComponent.RemoveFilter(Column);
        }

        protected void HandleDragStart()
        {
            var values = GridComponent.Grid.Settings.SortSettings.SortValues;
            var maxId = values.Any() ? values.Max(x => x.Id) + 1 : 1;
            GridComponent.Payload = new ColumnOrderValue(Column.Name, Column.Direction ?? GridSortDirection.Ascending, maxId);
        }

        private void HideFilter()
        {
            if (_isFilterVisible)
            {
                _isFilterVisible = false;
                StateHasChanged();
            }
        }

        protected async Task CheckboxChangeHandler()
        {        
            if (Column.HeaderCheckbox)
            {
                if(_allChecked.HasValue)
                    await SetChecked(!_allChecked.Value);
                else
                    await SetChecked(true);
            }
        }

        private async Task RowCheckboxChanged(CheckboxEventArgs<T> e)
        {
            if (e.ColumnName == Column.Name)
            {
                if (Column.HeaderCheckbox)
                {
                    if (GridComponent.ExceptCheckedRows.Get(Column.Name).Values.Where(r => r == true).Count() 
                        == GridComponent.Grid.ItemsCount
                        || (_allChecked == true &&
                            GridComponent.ExceptCheckedRows.Get(Column.Name).Values.Where(r => r == false).Count() == 0))
                    {
                        _allChecked = true;
                        _showAllChecked = true;
                        GridComponent.ExceptCheckedRows.AddParameter(Column.Name, new QueryDictionary<bool>());
                    }
                    else if (GridComponent.ExceptCheckedRows.Get(Column.Name).Values.Where(r => r == false).Count()
                        == GridComponent.Grid.ItemsCount
                        || (_allChecked == false &&
                            GridComponent.ExceptCheckedRows.Get(Column.Name).Values.Where(r => r == true).Count() == 0))
                    {
                        _allChecked = false;
                        _showAllChecked = true;
                        GridComponent.ExceptCheckedRows.AddParameter(Column.Name, new QueryDictionary<bool>());
                    }
                    else
                    {
                        _showAllChecked = false;
                    }
                }
                else
                {
                    _allChecked = null;
                    _showAllChecked = false;
                }
                StateHasChanged();
                await Task.CompletedTask;
            }
        }

        public bool? IsChecked()
        {
            return _allChecked;
        }

        public async Task SetChecked(bool value)
        {
            if (Column.HeaderCheckbox)
            {
                // set a value and init ExceptCheckedRows for this column
                _allChecked = value;
                _showAllChecked = true;
                GridComponent.ExceptCheckedRows.AddParameter(Column.Name, new QueryDictionary<bool>());

                CheckboxEventArgs<T> args = new CheckboxEventArgs<T>
                {
                    ColumnName = Column.Name
                };
                if (value)
                {
                    args.Value = CheckboxValue.Checked;
                }
                else
                {
                    args.Value = CheckboxValue.Unchecked;
                }
                await GridComponent.OnHeaderCheckboxChanged(args);
                StateHasChanged();
            }
        }
    }
}