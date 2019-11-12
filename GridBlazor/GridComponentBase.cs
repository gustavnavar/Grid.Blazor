using GridBlazor.Columns;
using GridBlazor.Pagination;
using GridBlazor.Searching;
using GridShared;
using GridShared.Columns;
using GridShared.Filtering;
using GridShared.Sorting;
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
        protected T _item;

        internal int SelectedRow { get; set; } = -1;
        internal ICGridColumn FirstColumn { get; set; }

        internal ColumnOrderValue Payload { get; set; }

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

            _hasSubGrid = Grid.SubGridKeys != null && Grid.SubGridKeys.Length > 0;
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
            Grid.AddQueryParameter(GridPager.DefaultPageQueryParameter, page.ToString());
            await UpdateGrid();
        }

        public async Task GetSortUrl(string columnQueryParameterName, string columnName, 
            string directionQueryParameterName, string direction)
        {
            Grid.AddQueryParameter(columnQueryParameterName, columnName);
            Grid.AddQueryParameter(directionQueryParameterName, direction);
            await UpdateGrid();
        }

        public async Task AddFilter(IGridColumn column, FilterCollection filters)
        {
            Grid.AddFilterParameter(column, filters);
            await UpdateGrid();
        }

        public async Task RemoveFilter(IGridColumn column)
        {
            Grid.RemoveFilterParameter(column);
            await UpdateGrid();
        }

        public async Task AddSearch(string searchValue)
        {
            Grid.AddQueryParameter(QueryStringSearchSettings.DefaultSearchQueryParameter, searchValue);
            await UpdateGrid();
        }

        public async Task RemoveSearch()
        {
            Grid.RemoveQueryParameter(QueryStringSearchSettings.DefaultSearchQueryParameter);
            await UpdateGrid();
        }

        public async Task AddExtSorting()
        {
            Grid.AddQueryString(ColumnOrderValue.DefaultSortingQueryParameter, Payload.ToString());
            await UpdateGrid();
        }

        public async Task ChangeExtSorting(ColumnOrderValue column)
        {
            var newColumnOrderValue = new ColumnOrderValue { 
                ColumnName = column.ColumnName,
                Direction = column.Direction == GridSortDirection.Ascending ? GridSortDirection.Descending 
                    : GridSortDirection.Ascending,
                Id = column.Id
            };
            Grid.ChangeQueryString(ColumnOrderValue.DefaultSortingQueryParameter, column.ToString(), 
                newColumnOrderValue.ToString());
            await UpdateGrid();
        }

        public async Task RemoveExtSorting(ColumnOrderValue column)
        {
            Grid.RemoveQueryString(ColumnOrderValue.DefaultSortingQueryParameter, column.ToString());
            await UpdateGrid();
        }

        public async Task CreateHandler()
        {
            _item = (T)Activator.CreateInstance(typeof(T));
            ((CGrid<T>)Grid).Mode = GridMode.Create;
            await UpdateGrid();
        }

        public void ReadHandler(object item)
        {
            _item = (T)item;
            ((CGrid<T>)Grid).Mode = GridMode.Read;
            StateHasChanged();
        }

        public async Task UpdateHandler(object item)
        {
            var keys = Grid.GetPrimaryKeyValues(item);
            _item = await ((CGrid<T>)Grid).CrudDataService.Get(keys);
            ((CGrid<T>)Grid).Mode = GridMode.Update;
            StateHasChanged();
        }

        public void DeleteHandler(object item)
        {
            _item = (T)item;
            ((CGrid<T>)Grid).Mode = GridMode.Delete;
            StateHasChanged();
        }

        public void BackButton()
        {
            ((CGrid<T>)Grid).Mode = GridMode.Grid;
            StateHasChanged();
        }

        public async Task CreateItem()
        {
            await ((CGrid<T>)Grid).CrudDataService.Insert(_item);
            ((CGrid<T>)Grid).Mode = GridMode.Grid;
            await UpdateGrid();
        }

        public async Task UpdateItem()
        {
            await ((CGrid<T>)Grid).CrudDataService.Update(_item);
            ((CGrid<T>)Grid).Mode = GridMode.Grid;
            await UpdateGrid();
        }

        public async Task DeleteItem()
        {
            var keys = Grid.GetPrimaryKeyValues(_item);
            await ((CGrid<T>)Grid).CrudDataService.Delete(keys);
            ((CGrid<T>)Grid).Mode = GridMode.Grid;
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
