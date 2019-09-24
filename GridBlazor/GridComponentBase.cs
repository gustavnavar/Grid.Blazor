using GridBlazor.Columns;
using GridBlazor.Pagination;
using GridBlazor.Searching;
using GridShared.Columns;
using GridShared.Filtering;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GridBlazor
{
    public class GridComponentBase<T> : ComponentBase
    {
        protected bool _hasSubGrid = false;
        protected bool _hasTotals = false;
        protected bool _requiredTotalsColumn = false;
        internal bool[] IsSubGridVisible;
        internal bool[] InitSubGrid;
        protected IQueryDictionary<Type> _filterComponents;

        internal int SelectedRow { get; set; } = -1;
        internal ICGridColumn FirstColumn { get; set; }

        [Parameter]
        public ICGrid Grid { get; set; }

        [Parameter]
        public Action<object> OnRowClicked { get; set; }

        [Parameter]
        public IQueryDictionary<Type> CustomFilters { get; set; }

        protected override void OnParametersSet()
        {
            _filterComponents = new QueryDictionary<Type>();
            _filterComponents.Add("System.String", typeof(TextFilterComponent<T>));
            _filterComponents.Add("System.Int32", typeof(NumberFilterComponent<T>));
            _filterComponents.Add("System.Double", typeof(NumberFilterComponent<T>));
            _filterComponents.Add("System.Decimal", typeof(NumberFilterComponent<T>));
            _filterComponents.Add("System.Byte", typeof(NumberFilterComponent<T>));
            _filterComponents.Add("System.Single", typeof(NumberFilterComponent<T>));
            _filterComponents.Add("System.Float", typeof(NumberFilterComponent<T>));
            _filterComponents.Add("System.Int64", typeof(NumberFilterComponent<T>));
            _filterComponents.Add("System.Int16", typeof(NumberFilterComponent<T>));
            _filterComponents.Add("System.UInt64", typeof(NumberFilterComponent<T>));
            _filterComponents.Add("System.UInt32", typeof(NumberFilterComponent<T>));
            _filterComponents.Add("System.UInt16", typeof(NumberFilterComponent<T>));
            _filterComponents.Add("System.DateTime", typeof(DateTimeFilterComponent<T>));
            _filterComponents.Add("System.Date", typeof(DateTimeFilterComponent<T>));
            _filterComponents.Add("System.DateTimeOffset", typeof(DateTimeFilterComponent<T>));
            _filterComponents.Add("System.Boolean", typeof(BooleanFilterComponent<T>));

            if (CustomFilters != null)
            {
                foreach(var widget in CustomFilters)
                {
                    if (_filterComponents.ContainsKey(widget.Key))
                        _filterComponents[widget.Key] = widget.Value;
                    else
                        _filterComponents.Add(widget);
                }
            }

            FirstColumn = (ICGridColumn)Grid.Columns.FirstOrDefault();

            _hasSubGrid = Grid.Keys != null && Grid.Keys.Length > 0;
            _hasTotals = Grid.IsSumEnabled || Grid.IsAverageEnabled || Grid.IsMaxEnabled || Grid.IsMinEnabled;
            _requiredTotalsColumn = _hasTotals
                && FirstColumn != null
                && (FirstColumn.IsSumEnabled || FirstColumn.IsAverageEnabled
                    || FirstColumn.IsMaxEnabled || FirstColumn.IsMinEnabled);

            if (_hasSubGrid && (IsSubGridVisible == null || IsSubGridVisible.Length != Grid.DisplayingItemsCount))
            {
                IsSubGridVisible = new bool[Grid.DisplayingItemsCount];
                for (int i = 0; i < IsSubGridVisible.Length; i++)
                {
                    IsSubGridVisible[i] = false;
                }
            }
            if (_hasSubGrid && (InitSubGrid == null || InitSubGrid.Length != Grid.DisplayingItemsCount))
            {
                InitSubGrid = new bool[Grid.DisplayingItemsCount];
                for (int i = 0; i < InitSubGrid.Length; i++)
                {
                    InitSubGrid[i] = true;
                }
            }
        }

        protected void RowClicked(int i, object item)
        {
            SelectedRow = i;
            if (OnRowClicked != null)
                OnRowClicked.Invoke(item);
        }

        protected void SubGridClicked(int i)
        {
            IsSubGridVisible[i] = !IsSubGridVisible[i];
        }

        public async Task GoTo(int page)
        {
            ((CGrid<T>)Grid).AddQueryParameter(GridPager.DefaultPageQueryParameter, page.ToString());
            await UpdateGrid();
        }

        public async Task GetSortUrl(string columnQueryParameterName, string columnName, 
            string directionQueryParameterName, string direction)
        {
            ((CGrid<T>)Grid).AddQueryParameter(columnQueryParameterName, columnName);
            ((CGrid<T>)Grid).AddQueryParameter(directionQueryParameterName, direction);
            await UpdateGrid();
        }

        public async Task AddFilter(IGridColumn column, FilterCollection filters)
        {
            ((CGrid<T>)Grid).AddFilterParameter(column, filters);
            await UpdateGrid();
        }

        public async Task RemoveFilter(IGridColumn column)
        {
            ((CGrid<T>)Grid).RemoveFilterParameter(column);
            await UpdateGrid();
        }

        public async Task AddSearch(string searchValue)
        {
            ((CGrid<T>)Grid).AddQueryParameter(QueryStringSearchSettings.DefaultSearchQueryParameter, searchValue);
            await UpdateGrid();
        }

        public async Task RemoveSearch()
        {
            ((CGrid<T>)Grid).RemoveQueryParameter(QueryStringSearchSettings.DefaultSearchQueryParameter);
            await UpdateGrid();
        }

        public async Task UpdateGrid()
        {
            SelectedRow = -1;
            if (_hasSubGrid)
            {
                for (int i = 0; i < IsSubGridVisible.Length; i++)
                {
                    IsSubGridVisible[i] = false;
                }
                for (int i = 0; i < InitSubGrid.Length; i++)
                {
                    InitSubGrid[i] = true;
                }
            }
            await Grid.UpdateGrid();
            StateHasChanged();
        }
    }
}
