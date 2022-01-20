using GridShared;
using GridShared.Filtering;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace GridBlazor.Pages
{
    public partial class ListFilterComponent<T> : IDisposable
    {
        protected Timer _timer;
        protected bool _clearVisible = false;
        protected List<Filter> _filters;
        protected int _offset = 0;
        protected bool _initPosition = false;
        protected IEnumerable<SelectItem> _selectList = new List<SelectItem>();
        protected IEnumerable<SelectItem> _visibleList = new List<SelectItem>();
        protected ListFilterOptions _filterOptions;
        protected bool _includeIsNull => _filterOptions.IncludeIsNull;
        protected bool _includeIsNotNull => _filterOptions.IncludeIsNotNull;

        protected string SearchFilterListText { get; set; }

        protected ElementReference listFilter;

        [Inject]
        private IJSRuntime jSRuntime { get; set; }

        [CascadingParameter(Name = "GridHeaderComponent")]
        private GridHeaderComponent<T> GridHeaderComponent { get; set; }

        [Parameter]
        public bool Visible { get; set; }

        [Parameter]
        public string ColumnName { get; set; }

        [Parameter]
        public IEnumerable<ColumnFilterValue> FilterSettings { get; set; }

        public ListFilterComponent()
        {
            _timer = new Timer { Enabled = true, AutoReset = false, };
            _timer.Elapsed += TimerOnElapsed;
        }

        protected override void OnParametersSet()
        {
            var filterSettings = FilterSettings.Where(r => r != ColumnFilterValue.Null
               && r.FilterType != GridFilterType.Condition).Select(r =>
                   new Filter(r.FilterType.ToString("d"), r.FilterValue)).ToList();
            _clearVisible = filterSettings.Count() > 0;
            _filters = filterSettings.ToList();

            if (GridHeaderComponent.Column.FilterWidgetData.GetType() == typeof((IEnumerable<SelectItem>, ListFilterOptions)))
            {
                (_selectList, _filterOptions) = ((IEnumerable<SelectItem>, ListFilterOptions))GridHeaderComponent.Column.FilterWidgetData;
                _visibleList = _selectList;
                _timer.Interval = _filterOptions.SearchInputDebounceMilliseconds;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!_initPosition && listFilter.Id != null)
            {
                await jSRuntime.InvokeVoidAsync("gridJsFunctions.focusElement", listFilter);
                ScreenPosition sp = await jSRuntime.InvokeAsync<ScreenPosition>("gridJsFunctions.getPosition", listFilter);
                ScreenPosition gridTableSP = await jSRuntime.InvokeAsync<ScreenPosition>("gridJsFunctions.getPosition", GridHeaderComponent.GridComponent.GridTable);
                if (sp != null && gridTableSP != null)
                {
                    if (gridTableSP.X + gridTableSP.Width < sp.X + sp.Width)
                    {
                        _offset = gridTableSP.X + gridTableSP.Width - sp.X - sp.Width;
                        StateHasChanged();
                    }
                    else if (sp.X < gridTableSP.X)
                    {
                        _offset = gridTableSP.X - sp.X;
                        StateHasChanged();
                    }
                }
                _initPosition = true;
            }
        }

        private void MyClickHandler(MouseEventArgs e, bool isChecked, string value)
        {
            if (isChecked)
            {
                RemoveColumnFilterValue(value);
            }
            else
            {
                AddColumnFilterValue(value);
            }
            StateHasChanged();
        }

        private void IsNullHandler(MouseEventArgs e, bool isChecked)
        {
            if (isChecked)
            {
                var filters = _filters.Where(r => r.Type == GridFilterType.IsNull.ToString("d"));
                for (int i = filters.Count() - 1; i >= 0; i--)
                    _filters.Remove(filters.ElementAt(i));
            }
            else
            {
                _filters.Add(new Filter(GridFilterType.IsNull.ToString("d"), ""));
            }
            StateHasChanged();
        }

        private void IsNotNullHandler(MouseEventArgs e, bool isChecked)
        {
            if (isChecked)
            {
                var filters = _filters.Where(r => r.Type == GridFilterType.IsNotNull.ToString("d"));
                for (int i = filters.Count() - 1; i >= 0; i--)
                    _filters.Remove(filters.ElementAt(i));
            }
            else
            {
                _filters.Add(new Filter(GridFilterType.IsNotNull.ToString("d"), ""));
            }
            StateHasChanged();
        }

        protected void AddColumnFilterValue(string value)
        {
            _filters.Add(new Filter(GridFilterType.Equals.ToString("d"), value));
        }

        protected void RemoveColumnFilterValue(string value)
        {
            var filters = _filters.Where(r => r.Value.Equals(value));
            for (int i = filters.Count() - 1; i >= 0; i--)
                _filters.Remove(filters.ElementAt(i));
        }

        protected async Task ApplyButtonClicked()
        {
            FilterCollection filters = new FilterCollection(_filters);
            if (filters.Count() > 1)
                filters.Add(GridFilterType.Condition.ToString("d"), "2");
            await GridHeaderComponent.AddFilter(filters);
        }

        protected Task SelectVisibleButtonClicked()
        {
            foreach (var item in _visibleList)
                AddColumnFilterValue(item.Value);

            StateHasChanged();
            return Task.CompletedTask;
        }

        protected Task UnselectVisibleButtonClicked()
        {
            if(string.IsNullOrEmpty(SearchFilterListText))
                _filters.Clear();
            else
                foreach (var item in _visibleList)
                    RemoveColumnFilterValue(item.Value);

            StateHasChanged();
            return Task.CompletedTask;
        }

        protected void FilterTextChanged(ChangeEventArgs e)
        {
            SearchFilterListText = e.Value?.ToString();
            _timer.Stop();
            _timer.Start();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
            => FilterVisibleItems(SearchFilterListText);

        protected void FilterVisibleItems(string text)
        {
            var selectedValues = _filters.Where(x => x.Type.Equals("1"));
            _visibleList = string.IsNullOrEmpty(text)
                ? _selectList
                : _selectList
                    .Where(item => selectedValues.Any(r => r.Value == item.Value) 
                        || item.Title?.Contains(text, _filterOptions.SearchComparisonMethod) == true)
                    .ToList();
            InvokeAsync(StateHasChanged);
        }

        protected async Task ClearButtonClicked()
        {
            await GridHeaderComponent.RemoveFilter();
        }

        public async Task FilterKeyup(KeyboardEventArgs e)
        {
            if (e.Key == "Escape")
            {
                await GridHeaderComponent.FilterIconClicked();
            }
        }

        void IDisposable.Dispose()
        {
            _timer?.Dispose();
            _timer = null;
        }
    }
}

