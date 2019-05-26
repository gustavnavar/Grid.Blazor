using GridBlazor.Pagination;
using GridBlazor.Searching;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace GridBlazor
{
    public class GridComponentBase<T> : ComponentBase
    {
        protected int _selectedRow = -1;
        protected IQueryDictionary<Type> _filterComponents;

        [Parameter]
        protected ICGrid<T> Grid { get; set; }

        [Parameter]
        protected Action<object> OnRowClicked { get; set; }

        [Parameter]
        protected IQueryDictionary<Type> CustomFilters { get; set; }

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
        }

        protected void RowClicked(int i, object item)
        {
            _selectedRow = i;
            if (OnRowClicked != null)
                OnRowClicked.Invoke(item);
            StateHasChanged();
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

        public async Task AddFilter(string columnName, string filterType, string filterValue)
        {
            ((CGrid<T>)Grid).AddFilterParameter(columnName, filterType, filterValue);
            await UpdateGrid();
        }

        public async Task RemoveFilter(string columnName)
        {
            ((CGrid<T>)Grid).RemoveFilterParameter(columnName);
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

        private async Task UpdateGrid()
        {
            _selectedRow = -1;
            await Grid.UpdateGrid();
            StateHasChanged();
        }
    }
}
